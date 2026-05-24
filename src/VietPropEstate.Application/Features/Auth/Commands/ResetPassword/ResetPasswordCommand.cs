using MediatR;

namespace VietPropEstate.Application.Features.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand : IRequest
{
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmNewPassword { get; init; } = string.Empty;
}
