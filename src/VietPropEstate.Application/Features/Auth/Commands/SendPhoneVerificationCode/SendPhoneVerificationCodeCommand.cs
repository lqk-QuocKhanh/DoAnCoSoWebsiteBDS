using MediatR;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.SendPhoneVerificationCode;

public sealed record SendPhoneVerificationCodeCommand : IRequest<SendPhoneVerificationResponseDto>
{
    public string UserId { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}
