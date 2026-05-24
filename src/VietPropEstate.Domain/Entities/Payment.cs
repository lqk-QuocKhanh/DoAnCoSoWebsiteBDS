using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Exceptions;
using VietPropEstate.Domain.ValueObjects;

namespace VietPropEstate.Domain.Entities;

/// <summary>A payment record — primarily for VIP package subscriptions via VNPay.</summary>
public class Payment : AuditableEntity
{
    public string UserId { get; private set; } = string.Empty;
    public Guid? UserVIPPackageId { get; private set; }
    public Guid? VIPPackageId { get; private set; }
    public Money Amount { get; private set; } = Money.Zero();
    public PaymentStatus Status { get; private set; }

    /// <summary>Our unique reference sent to VNPay as vnp_TxnRef (32-char hex GUID).</summary>
    public string PaymentReference { get; private set; } = string.Empty;

    /// <summary>Payment method identifier (e.g. "VNPay", "MoMo").</summary>
    public string? PaymentMethod { get; private set; }

    /// <summary>Human-readable description sent to VNPay as vnp_OrderInfo.</summary>
    public string? OrderInfo { get; private set; }

    /// <summary>VNPay's own transaction number (vnp_TransactionNo from callback).</summary>
    public string? VnpayTransactionId { get; private set; }

    /// <summary>Bank code used for payment (vnp_BankCode).</summary>
    public string? BankCode { get; private set; }

    /// <summary>Raw JSON/query-string of the VNPay callback for audit purposes.</summary>
    public string? GatewayResponse { get; private set; }

    /// <summary>Client IP address at time of payment initiation.</summary>
    public string? IpAddress { get; private set; }

    public DateTime? PaidAt { get; private set; }
    public string? Notes { get; private set; }

    public UserVIPPackage? UserVIPPackage { get; private set; }

    private Payment() { }

    public static Payment Create(
        string userId,
        Money amount,
        string paymentReference,
        string paymentMethod,
        string? orderInfo = null,
        string? ipAddress = null,
        Guid? vipPackageId = null,
        Guid? userVIPPackageId = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User ID is required for a payment.");
        if (string.IsNullOrWhiteSpace(paymentMethod))
            throw new DomainException("Payment method is required.");
        if (string.IsNullOrWhiteSpace(paymentReference))
            throw new DomainException("Payment reference is required.");

        return new Payment
        {
            UserId = userId,
            Amount = amount,
            PaymentReference = paymentReference,
            PaymentMethod = paymentMethod,
            OrderInfo = orderInfo,
            IpAddress = ipAddress,
            VIPPackageId = vipPackageId,
            UserVIPPackageId = userVIPPackageId,
            Status = PaymentStatus.Pending
        };
    }

    public void MarkAsProcessing() =>
        Status = Status == PaymentStatus.Pending
            ? PaymentStatus.Processing
            : throw new DomainException("Only pending payments can be moved to processing.");

    public void Complete(
        string transactionCode,
        string? vnpayTransactionId = null,
        string? bankCode = null,
        string? gatewayResponse = null)
    {
        if (Status is not (PaymentStatus.Pending or PaymentStatus.Processing))
            throw new DomainException("Payment cannot be completed from its current state.");

        Status = PaymentStatus.Completed;
        TransactionCode = transactionCode;
        VnpayTransactionId = vnpayTransactionId;
        BankCode = bankCode;
        GatewayResponse = gatewayResponse;
        PaidAt = DateTime.UtcNow;
    }

    public void Fail(string? reason = null, string? gatewayResponse = null)
    {
        if (Status is PaymentStatus.Completed or PaymentStatus.Refunded)
            throw new DomainException("Completed or refunded payments cannot be failed.");

        Status = PaymentStatus.Failed;
        Notes = reason;
        GatewayResponse ??= gatewayResponse;
    }

    public void Refund(string reason)
    {
        if (Status != PaymentStatus.Completed)
            throw new DomainException("Only completed payments can be refunded.");

        Status = PaymentStatus.Refunded;
        Notes = reason;
    }

    public void Cancel(string reason)
    {
        if (Status is PaymentStatus.Completed or PaymentStatus.Refunded)
            throw new DomainException("Completed or refunded payments cannot be cancelled.");

        Status = PaymentStatus.Cancelled;
        Notes = reason;
    }

    // Backing field for TransactionCode (needed since we add it outside Create)
    public string? TransactionCode { get; private set; }
}
