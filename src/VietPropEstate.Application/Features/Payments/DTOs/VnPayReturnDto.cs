namespace VietPropEstate.Application.Features.Payments.DTOs;

/// <summary>Parsed result shown to the user after VNPay redirect (Return URL).</summary>
public sealed class VnPayReturnDto
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid? PaymentId { get; init; }
    public decimal? AmountVnd { get; init; }
    public string? BankCode { get; init; }
    public string? TransactionId { get; init; }
    public string? PayDate { get; init; }
    public string ResponseCode { get; init; } = string.Empty;
}
