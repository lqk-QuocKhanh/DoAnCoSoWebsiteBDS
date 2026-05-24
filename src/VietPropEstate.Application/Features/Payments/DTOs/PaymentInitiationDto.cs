namespace VietPropEstate.Application.Features.Payments.DTOs;

/// <summary>Returned to the client after a payment is initiated.</summary>
public sealed class PaymentInitiationDto
{
    public Guid PaymentId { get; init; }
    public string PaymentReference { get; init; } = string.Empty;
    public string PaymentUrl { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "VND";
    public string? VIPPackageName { get; init; }
    public int ExpiresInMinutes { get; init; } = 15;
}
