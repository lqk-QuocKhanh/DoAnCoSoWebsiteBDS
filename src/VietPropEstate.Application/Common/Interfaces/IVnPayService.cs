namespace VietPropEstate.Application.Common.Interfaces;

/// <summary>VNPay payment gateway integration — URL generation and signature verification.</summary>
public interface IVnPayService
{
    /// <summary>
    /// Generates the redirect URL for VNPay checkout.
    /// </summary>
    /// <param name="txnRef">Our unique payment reference (32-char hex).</param>
    /// <param name="amountVnd">Amount in Vietnamese đồng (e.g. 500000 for 500,000 VND).</param>
    /// <param name="orderInfo">Description shown on VNPay payment page (max 255 chars).</param>
    /// <param name="ipAddress">Client's IP address.</param>
    /// <param name="locale">Payment page language: "vn" or "en". Default: "vn".</param>
    /// <param name="expireMinutes">How long the URL is valid. Default: 15 minutes.</param>
    string CreatePaymentUrl(
        string txnRef,
        decimal amountVnd,
        string orderInfo,
        string ipAddress,
        string locale = "vn",
        int expireMinutes = 15);

    /// <summary>
    /// Verifies the HMAC-SHA512 signature and parses VNPay callback/IPN parameters.
    /// Works for both Return URL (redirect) and IPN (server-to-server) callbacks.
    /// </summary>
    VnPayVerificationResult VerifyCallback(IEnumerable<KeyValuePair<string, string>> parameters);

    /// <summary>True when local mock checkout is used instead of the real VNPay gateway.</summary>
    bool UseMockPayment { get; }

    /// <summary>URL of the in-app mock payment page (development only).</summary>
    string CreateMockPaymentUrl(string txnRef, decimal amountVnd, string packageName);

    /// <summary>Builds signed callback parameters for mock payment completion.</summary>
    IReadOnlyList<KeyValuePair<string, string>> BuildMockCallbackParameters(
        string txnRef,
        decimal amountVnd,
        bool success);

    /// <summary>Builds the browser return URL with signed VNPay query parameters.</summary>
    string BuildBrowserReturnUrl(IEnumerable<KeyValuePair<string, string>> parameters);
}

/// <summary>Parsed and verified result from a VNPay callback or IPN.</summary>
public sealed record VnPayVerificationResult
{
    /// <summary>True if the HMAC-SHA512 signature matches.</summary>
    public bool IsSignatureValid { get; init; }

    /// <summary>True when ResponseCode == "00" AND TransactionStatus == "00".</summary>
    public bool IsSuccess { get; init; }

    /// <summary>Our payment reference (vnp_TxnRef) — used to look up the Payment record.</summary>
    public string TxnRef { get; init; } = string.Empty;

    /// <summary>VNPay's own transaction number (vnp_TransactionNo).</summary>
    public string? VnpayTransactionId { get; init; }

    /// <summary>Amount in VND returned by VNPay (vnp_Amount ÷ 100).</summary>
    public decimal AmountVnd { get; init; }

    /// <summary>Bank code used for payment.</summary>
    public string? BankCode { get; init; }

    /// <summary>Bank's own transaction reference.</summary>
    public string? BankTransactionNo { get; init; }

    /// <summary>VNPay response code: "00" = success.</summary>
    public string ResponseCode { get; init; } = string.Empty;

    /// <summary>VNPay transaction status: "00" = success.</summary>
    public string TransactionStatus { get; init; } = string.Empty;

    /// <summary>Payment date from VNPay (yyyyMMddHHmmss).</summary>
    public string? PayDate { get; init; }

    /// <summary>All raw parameters as a serialised string for audit logging.</summary>
    public string RawParameters { get; init; } = string.Empty;
}
