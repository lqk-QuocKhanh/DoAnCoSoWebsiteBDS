using MediatR;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IAuthService _authService;

    public ChangePasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        => _authService.ChangePasswordAsync(
            request.UserId, request.CurrentPassword, request.NewPassword, cancellationToken);
}
