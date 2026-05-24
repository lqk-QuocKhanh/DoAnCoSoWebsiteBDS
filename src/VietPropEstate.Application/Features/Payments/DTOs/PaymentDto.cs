using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Payments.DTOs;

public sealed class PaymentDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public Guid? VIPPackageId { get; init; }
    public string? VIPPackageName { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "VND";
    public PaymentStatus Status { get; init; }
    public string? PaymentMethod { get; init; }
    public string? OrderInfo { get; init; }
    public string PaymentReference { get; init; } = string.Empty;
    public string? TransactionCode { get; init; }
    public string? VnpayTransactionId { get; init; }
    public string? BankCode { get; init; }
    public DateTime? PaidAt { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
}
