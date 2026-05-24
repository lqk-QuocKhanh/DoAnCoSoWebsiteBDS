using MediatR;

namespace VietPropEstate.Application.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand : IRequest
{
    public string UserId { get; init; } = string.Empty;
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmNewPassword { get; init; } = string.Empty;
}
