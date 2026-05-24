using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Payments.Commands.ProcessVnPayIpn;

public sealed class ProcessVnPayIpnCommandHandler
    : IRequestHandler<ProcessVnPayIpnCommand, VnPayIpnResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IVnPayService _vnPay;
    private readonly INotificationService _notifications;
    private readonly ILogger<ProcessVnPayIpnCommandHandler> _logger;

    public ProcessVnPayIpnCommandHandler(
        IApplicationDbContext db,
        IVnPayService vnPay,
        INotificationService notifications,
        ILogger<ProcessVnPayIpnCommandHandler> logger)
    {
        _db = db;
        _vnPay = vnPay;
        _notifications = notifications;
        _logger = logger;
    }

    public async Task<VnPayIpnResponse> Handle(
        ProcessVnPayIpnCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify HMAC signature
        var result = _vnPay.VerifyCallback(request.Parameters);

        if (!result.IsSignatureValid)
        {
            _logger.LogWarning("VNPay IPN: invalid signature for TxnRef={TxnRef}", result.TxnRef);
            return new VnPayIpnResponse("97", "Invalid Signature");
        }

        // 2. Find payment by reference
        var payment = await _db.Payments
            .FirstOrDefaultAsync(p => p.PaymentReference == result.TxnRef, cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("VNPay IPN: payment not found for TxnRef={TxnRef}", result.TxnRef);
            return new VnPayIpnResponse("01", "Order not found");
        }

        // 3. Idempotency — already processed
        if (payment.Status == PaymentStatus.Completed)
        {
            _logger.LogInformation(
                "VNPay IPN: duplicate IPN for already-completed payment {Id}.", payment.Id);
            return new VnPayIpnResponse("02", "Order already confirmed");
        }

        // 4. Verify amount (VNPay returns amount × 100, we already divided by 100 in service)
        if (result.AmountVnd != payment.Amount.Amount)
        {
            _logger.LogWarning(
                "VNPay IPN: amount mismatch. Expected={Expected}, Got={Got}. TxnRef={TxnRef}",
                payment.Amount.Amount, result.AmountVnd, result.TxnRef);
            return new VnPayIpnResponse("04", "Invalid Amount");
        }

        // 5. Process result
        if (result.IsSuccess)
        {
            await HandleSuccessAsync(payment, result, cancellationToken);
        }
        else
        {
            _logger.LogInformation(
                "VNPay IPN: payment failed. TxnRef={TxnRef}, ResponseCode={Code}",
                result.TxnRef, result.ResponseCode);

            payment.Fail(
                $"VNPay ResponseCode={result.ResponseCode}",
                result.RawParameters);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return new VnPayIpnResponse("00", "Confirm Success");
    }

    // ─── Private helpers ─────────────────────────────────────────────────────

    private async Task HandleSuccessAsync(
        Payment payment,
        VnPayVerificationResult result,
        CancellationToken cancellationToken)
    {
        payment.Complete(
            transactionCode: result.TxnRef,
            vnpayTransactionId: result.VnpayTransactionId,
            bankCode: result.BankCode,
            gatewayResponse: result.RawParameters);

        // Activate VIP subscription if this is a package payment
        if (payment.VIPPackageId.HasValue)
        {
            var package = await _db.VIPPackages
                .FirstOrDefaultAsync(p => p.Id == payment.VIPPackageId.Value, cancellationToken);

            if (package is not null)
            {
                var subscription = UserVIPPackage.Create(
                    payment.UserId, package, DateTime.UtcNow);

                await _db.UserVIPPackages.AddAsync(subscription, cancellationToken);

                _logger.LogInformation(
                    "VIP package {Name} activated for user {UserId}. Expires {End}.",
                    package.Name, payment.UserId, subscription.EndDate);

                // Send in-app notification
                try
                {
                    await _notifications.CreateAndPushAsync(
                        payment.UserId,
                        "Thanh toán thành công",
                        $"Gói VIP {package.Name} đã được kích hoạt. Hiệu lực đến {subscription.EndDate:dd/MM/yyyy}.",
                        NotificationType.VIPPackageActivated,
                        referenceId: subscription.Id.ToString(),
                        actionUrl: "/account/subscriptions",
                        cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send VIP activation notification.");
                }
            }
        }

        _logger.LogInformation(
            "Payment {Id} completed successfully. VNPayTxn={TxnId}, Bank={Bank}",
            payment.Id, result.VnpayTransactionId, result.BankCode);
    }
}
