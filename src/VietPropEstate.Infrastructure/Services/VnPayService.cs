using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Infrastructure.Services;

/// <summary>
/// VNPay payment gateway integration (API version 2.1.0).
/// Docs: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html
///
/// Security model:
///   - Outgoing: sort params alphabetically, HMAC-SHA512 with HashSecret
///   - Incoming: verify HMAC-SHA512 before processing any IPN/return callback
/// </summary>
public sealed class VnPayService : IVnPayService
{
    private readonly VnPaySettings _settings;
    private readonly ILogger<VnPayService> _logger;

    // VNPay uses Vietnam time (UTC+7) for date parameters
    private static readonly TimeZoneInfo VietnamTz =
        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

    public VnPayService(IConfiguration configuration, ILogger<VnPayService> logger)
    {
        _settings = configuration
            .GetSection("VnPay")
            .Get<VnPaySettings>()
            ?? throw new InvalidOperationException("VnPay settings are not configured.");

        _logger = logger;
    }

    public bool UseMockPayment => _settings.UseMockPayment;

    // ── Mock payment (local development) ──────────────────────────────────────

    public string CreateMockPaymentUrl(string txnRef, decimal amountVnd, string packageName)
    {
        var baseUrl = _settings.MockPaymentBaseUrl.TrimEnd('/');
        return $"{baseUrl}/payment/vnpay-mock" +
               $"?ref={Uri.EscapeDataString(txnRef)}" +
               $"&amount={amountVnd.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
               $"&package={Uri.EscapeDataString(packageName)}";
    }

    public IReadOnlyList<KeyValuePair<string, string>> BuildMockCallbackParameters(
        string txnRef,
        decimal amountVnd,
        bool success)
    {
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTz);
        var amountMinor = ((long)(amountVnd * 100)).ToString();

        var signParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Amount"]            = amountMinor,
            ["vnp_BankCode"]          = success ? "NCB" : "MOCK",
            ["vnp_OrderInfo"]         = "Mock VNPay payment",
            ["vnp_PayDate"]           = now.ToString("yyyyMMddHHmmss"),
            ["vnp_ResponseCode"]      = success ? "00" : "24",
            ["vnp_TmnCode"]           = GetEffectiveTmnCode(),
            ["vnp_TransactionNo"]     = success ? Random.Shared.Next(10_000_000, 99_999_999).ToString() : "0",
            ["vnp_TransactionStatus"] = success ? "00" : "02",
            ["vnp_TxnRef"]            = txnRef,
        };

        var queryString = BuildQueryString(signParams);
        var secureHash = ComputeHmacSha512(_settings.HashSecret, queryString);

        var result = signParams.ToList();
        result.Add(new KeyValuePair<string, string>("vnp_SecureHash", secureHash));
        return result;
    }

    public string BuildBrowserReturnUrl(IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var paramList = parameters.ToList();
        var signParams = paramList
            .Where(kv => !kv.Key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase))
            .OrderBy(kv => kv.Key, StringComparer.Ordinal)
            .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal);

        var queryString = BuildQueryString(signParams);
        var secureHash = paramList
            .FirstOrDefault(kv => kv.Key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase))
            .Value;

        return $"{_settings.ReturnUrl}?{queryString}&vnp_SecureHash={secureHash}";
    }

    // ── Payment URL generation ───────────────────────────────────────────────

    public string CreatePaymentUrl(
        string txnRef,
        decimal amountVnd,
        string orderInfo,
        string ipAddress,
        string locale = "vn",
        int expireMinutes = 15)
    {
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTz);
        var expire = now.AddMinutes(expireMinutes);

        // VNPay amount is in minor units: 500,000 VND → 50,000,000
        var vnpAmount = ((long)(amountVnd * 100)).ToString();

        var @params = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"]    = _settings.Version,
            ["vnp_Command"]    = _settings.Command,
            ["vnp_TmnCode"]    = GetEffectiveTmnCode(),
            ["vnp_Amount"]     = vnpAmount,
            ["vnp_CurrCode"]   = "VND",
            ["vnp_TxnRef"]     = txnRef,
            ["vnp_OrderInfo"]  = orderInfo,
            ["vnp_OrderType"]  = "other",
            ["vnp_Locale"]     = locale,
            ["vnp_ReturnUrl"]  = _settings.ReturnUrl,
            ["vnp_IpAddr"]     = ipAddress,
            ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss"),
            ["vnp_ExpireDate"] = expire.ToString("yyyyMMddHHmmss"),
        };

        if (ShouldIncludeIpnUrl())
            @params["vnp_IpnUrl"] = _settings.IpnUrl;

        var queryString = BuildQueryString(@params);
        var secureHash = ComputeHmacSha512(_settings.HashSecret, queryString);

        var url = $"{_settings.BaseUrl}?{queryString}&vnp_SecureHash={secureHash}";

        _logger.LogInformation(
            "VNPay URL created for TxnRef={TxnRef}, Amount={Amount} VND.",
            txnRef, amountVnd);

        return url;
    }

    // ── Callback / IPN verification ─────────────────────────────────────────

    public VnPayVerificationResult VerifyCallback(
        IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var paramList = parameters.ToList();
        var paramDict = paramList.ToDictionary(kv => kv.Key, kv => kv.Value,
            StringComparer.OrdinalIgnoreCase);

        // Extract secure hash before building verification string
        paramDict.TryGetValue("vnp_SecureHash", out var secureHash);
        paramDict.TryGetValue("vnp_SecureHashType", out _);

        // Build sorted query string (excluding hash params)
        var filteredParams = paramDict
            .Where(kv => !kv.Key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase) &&
                         !kv.Key.Equals("vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal);

        var signParams = new SortedDictionary<string, string>(filteredParams, StringComparer.Ordinal);

        var queryString = BuildQueryString(signParams);
        var expectedHash = ComputeHmacSha512(_settings.HashSecret, queryString);

        var isSignatureValid = !string.IsNullOrEmpty(secureHash) &&
            string.Equals(expectedHash, secureHash, StringComparison.OrdinalIgnoreCase);

        if (!isSignatureValid)
            _logger.LogWarning("VNPay callback signature mismatch. TxnRef={TxnRef}",
                paramDict.GetValueOrDefault("vnp_TxnRef"));

        paramDict.TryGetValue("vnp_ResponseCode",     out var responseCode);
        paramDict.TryGetValue("vnp_TransactionStatus",out var txnStatus);
        paramDict.TryGetValue("vnp_TxnRef",           out var txnRef);
        paramDict.TryGetValue("vnp_TransactionNo",    out var transactionNo);
        paramDict.TryGetValue("vnp_Amount",           out var amountStr);
        paramDict.TryGetValue("vnp_BankCode",         out var bankCode);
        paramDict.TryGetValue("vnp_BankTranNo",       out var bankTranNo);
        paramDict.TryGetValue("vnp_PayDate",          out var payDate);

        decimal amountVnd = 0;
        if (long.TryParse(amountStr, out var amountMinor))
            amountVnd = amountMinor / 100m;

        var rawParams = JsonSerializer.Serialize(paramDict);

        return new VnPayVerificationResult
        {
            IsSignatureValid   = isSignatureValid,
            IsSuccess          = isSignatureValid &&
                                 responseCode == "00" &&
                                 txnStatus    == "00",
            TxnRef             = txnRef             ?? string.Empty,
            VnpayTransactionId = transactionNo,
            AmountVnd          = amountVnd,
            BankCode           = bankCode,
            BankTransactionNo  = bankTranNo,
            ResponseCode       = responseCode       ?? string.Empty,
            TransactionStatus  = txnStatus          ?? string.Empty,
            PayDate            = payDate,
            RawParameters      = rawParams
        };
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Builds the VNPay query string: sorted params, URL-encoded values, joined with &amp;
    /// The key is NOT URL-encoded (VNPay spec).
    /// </summary>
    private static string BuildQueryString(IDictionary<string, string> @params)
        => string.Join('&', @params.Select(kv =>
            $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

    /// <summary>HMAC-SHA512 — returns lowercase hex string.</summary>
    private static string ComputeHmacSha512(string key, string data)
    {
        var keyBytes  = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA512(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private string GetEffectiveTmnCode()
        => string.IsNullOrWhiteSpace(_settings.TmnCode) ? "MOCKDEV1" : _settings.TmnCode;

    private bool ShouldIncludeIpnUrl()
        => !string.IsNullOrWhiteSpace(_settings.IpnUrl) &&
           !_settings.IpnUrl.Contains("your-api-domain", StringComparison.OrdinalIgnoreCase);

    // ── Settings ─────────────────────────────────────────────────────────────

    private sealed class VnPaySettings
    {
        public string TmnCode              { get; init; } = string.Empty;
        public string HashSecret           { get; init; } = string.Empty;
        public string BaseUrl              { get; init; } = string.Empty;
        public string ReturnUrl            { get; init; } = string.Empty;
        public string IpnUrl               { get; init; } = string.Empty;
        public string Version              { get; init; } = "2.1.0";
        public string Command              { get; init; } = "pay";
        public bool UseMockPayment         { get; init; }
        public string MockPaymentBaseUrl   { get; init; } = "http://localhost:5126";
    }
}
