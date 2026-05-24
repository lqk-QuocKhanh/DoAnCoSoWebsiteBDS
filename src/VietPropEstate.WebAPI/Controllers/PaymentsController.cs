using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Payments.Commands.CancelPayment;
using VietPropEstate.Application.Features.Payments.Commands.InitiateVipPayment;
using VietPropEstate.Application.Features.Payments.Commands.ProcessVnPayIpn;
using VietPropEstate.Application.Features.Payments.DTOs;
using VietPropEstate.Application.Features.Payments.Queries.GetPaymentById;
using VietPropEstate.Application.Features.Payments.Queries.GetPaymentHistory;
using VietPropEstate.Domain.Enums;
using VietPropEstate.WebAPI.Authorization;
using VietPropEstate.WebAPI.Extensions;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>
/// VNPay payment integration.
/// Flow:
///   1. POST /api/payments/initiate  → returns PaymentUrl, redirect user to VNPay
///   2. VNPay → POST /api/payments/vnpay-ipn  (server-to-server, authoritative)
///   3. VNPay → GET  /api/payments/vnpay-return (user browser redirect, display only)
/// </summary>
public class PaymentsController : BaseApiController
{
    private readonly ICurrentUserService _currentUser;
    private readonly IVnPayService _vnPay;
    private readonly IApplicationDbContext _db;

    public PaymentsController(
        ICurrentUserService currentUser,
        IVnPayService vnPay,
        IApplicationDbContext db)
    {
        _currentUser = currentUser;
        _vnPay = vnPay;
        _db = db;
    }

    // ── POST /api/payments/initiate ───────────────────────────────────────────

    /// <summary>
    /// Initiates a VIP package payment.
    /// Returns a VNPay checkout URL that the client must redirect the user to.
    /// </summary>
    [HttpPost("initiate")]
    [Authorize(Policy = AuthPolicies.BrokerOnly)]
    [ProducesResponseType(typeof(PaymentInitiationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Initiate(
        [FromBody] InitiatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new InitiateVipPaymentCommand
        {
            UserId = _currentUser.UserId!,
            VIPPackageId = request.VIPPackageId,
            IpAddress = HttpContext.GetClientIpAddress(),
            Locale = request.Locale ?? "vn"
        }, cancellationToken);

        return Ok(result);
    }

    // ── POST /api/payments/vnpay-ipn ──────────────────────────────────────────

    /// <summary>
    /// VNPay Instant Payment Notification (IPN) endpoint.
    /// Called server-to-server by VNPay. Must return specific JSON responses.
    /// IMPORTANT: This endpoint must NOT require authentication.
    /// Configure IPN URL in VNPay merchant portal to point here.
    /// </summary>
    [HttpPost("vnpay-ipn")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> VnPayIpn(CancellationToken cancellationToken)
    {
        var parameters = Request.Query
            .Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value.ToString()))
            .ToList();

        var response = await Mediator.Send(
            new ProcessVnPayIpnCommand { Parameters = parameters },
            cancellationToken);

        // VNPay expects: { "RspCode": "00", "Message": "..." }
        return Ok(response);
    }

    // ── GET /api/payments/vnpay-return ────────────────────────────────────────

    /// <summary>
    /// VNPay Return URL — called when user is redirected back from VNPay.
    /// Display-only; authoritative confirmation happens via IPN.
    /// </summary>
    [HttpGet("vnpay-return")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(VnPayReturnDto), StatusCodes.Status200OK)]
    public IActionResult VnPayReturn()
    {
        var parameters = Request.Query
            .Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value.ToString()))
            .ToList();

        var result = _vnPay.VerifyCallback(parameters);

        var dto = new VnPayReturnDto
        {
            IsSuccess     = result.IsSuccess,
            ResponseCode  = result.ResponseCode,
            Message       = result.IsSuccess
                            ? "Thanh toán thành công. Gói VIP sẽ được kích hoạt trong giây lát."
                            : MapResponseMessage(result.ResponseCode),
            AmountVnd     = result.AmountVnd,
            BankCode      = result.BankCode,
            TransactionId = result.VnpayTransactionId,
            PayDate       = result.PayDate
        };

        return Ok(dto);
    }

    // ── POST /api/payments/vnpay-mock-complete ────────────────────────────────

    /// <summary>
    /// Completes a mock VNPay payment in development (UseMockPayment=true).
    /// Processes IPN server-side and returns the browser redirect URL.
    /// </summary>
    [HttpPost("vnpay-mock-complete")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VnPayMockComplete(
        [FromBody] MockVnPayCompleteRequest request,
        CancellationToken cancellationToken)
    {
        if (!_vnPay.UseMockPayment)
            return NotFound();

        var payment = await _db.Payments
            .FirstOrDefaultAsync(p => p.PaymentReference == request.PaymentReference, cancellationToken);

        if (payment is null)
            return NotFound(new { message = "Không tìm thấy giao dịch." });

        var parameters = _vnPay.BuildMockCallbackParameters(
            payment.PaymentReference,
            payment.Amount.Amount,
            request.Success);

        await Mediator.Send(
            new ProcessVnPayIpnCommand { Parameters = parameters },
            cancellationToken);

        return Ok(new MockVnPayCompleteResponse
        {
            RedirectUrl = _vnPay.BuildBrowserReturnUrl(parameters)
        });
    }

    // ── GET /api/payments ─────────────────────────────────────────────────────

    /// <summary>Returns the authenticated user's payment history (paginated).</summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] PaymentStatus? status = null,
        CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(new GetPaymentHistoryQuery
        {
            UserId = _currentUser.UserId!,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Status = status
        }, cancellationToken));

    // ── GET /api/payments/{id:guid} ───────────────────────────────────────────

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await Mediator.Send(
            new GetPaymentByIdQuery(id, _currentUser.UserId!), cancellationToken));

    // ── DELETE /api/payments/{id:guid} ────────────────────────────────────────

    /// <summary>Cancel a pending payment.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelPaymentRequest request,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new CancelPaymentCommand(id, _currentUser.UserId!, request.Reason),
            cancellationToken);
        return NoContent();
    }

    // ─── Private ─────────────────────────────────────────────────────────────

    private static string MapResponseMessage(string responseCode) => responseCode switch
    {
        "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
        "09" => "Thẻ/Tài khoản chưa đăng ký dịch vụ Internet Banking.",
        "10" => "Xác thực thông tin thẻ/tài khoản không đúng quá 3 lần.",
        "11" => "Đã hết hạn chờ thanh toán. Vui lòng thực hiện lại giao dịch.",
        "12" => "Thẻ/Tài khoản bị khóa.",
        "13" => "Nhập sai mật khẩu OTP. Vui lòng thực hiện lại giao dịch.",
        "24" => "Bạn đã hủy thanh toán.",
        "51" => "Tài khoản không đủ số dư để thực hiện giao dịch.",
        "65" => "Tài khoản đã vượt quá hạn mức giao dịch trong ngày.",
        "75" => "Ngân hàng thanh toán đang bảo trì.",
        "79" => "Nhập sai mật khẩu thanh toán quá số lần quy định.",
        "99" => "Lỗi không xác định. Vui lòng liên hệ hỗ trợ.",
        _    => $"Giao dịch thất bại (Mã lỗi: {responseCode})."
    };
}

// ── Request models ─────────────────────────────────────────────────────────────
public record InitiatePaymentRequest(Guid VIPPackageId, string? Locale);
public record CancelPaymentRequest(string Reason);
public record MockVnPayCompleteRequest(string PaymentReference, bool Success);
public sealed record MockVnPayCompleteResponse
{
    public string RedirectUrl { get; init; } = string.Empty;
}
