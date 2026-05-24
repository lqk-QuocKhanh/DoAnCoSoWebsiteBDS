using MediatR;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IAuthService _authService;

    public ResetPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        => _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword, cancellationToken);
}
