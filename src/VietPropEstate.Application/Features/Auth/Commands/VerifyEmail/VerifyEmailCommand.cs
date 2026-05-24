using MediatR;

namespace VietPropEstate.Application.Features.Auth.Commands.VerifyEmail;

public sealed record VerifyEmailCommand : IRequest
{
    public string UserId { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}
