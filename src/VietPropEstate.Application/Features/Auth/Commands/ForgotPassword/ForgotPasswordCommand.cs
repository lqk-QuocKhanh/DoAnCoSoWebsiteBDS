using MediatR;

namespace VietPropEstate.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand : IRequest
{
    public string Email { get; init; } = string.Empty;
}
