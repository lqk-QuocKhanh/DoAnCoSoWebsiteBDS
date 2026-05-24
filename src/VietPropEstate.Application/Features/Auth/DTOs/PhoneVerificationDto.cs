namespace VietPropEstate.Application.Features.Auth.DTOs;

public sealed record SendPhoneVerificationResponseDto
{
    public string Message { get; init; } = string.Empty;
    public string? DevCode { get; init; }
}
