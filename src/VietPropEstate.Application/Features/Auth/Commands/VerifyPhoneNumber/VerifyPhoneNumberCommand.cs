using MediatR;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.VerifyPhoneNumber;

public sealed record VerifyPhoneNumberCommand : IRequest<UserProfileDto>
{
    public string UserId { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}
