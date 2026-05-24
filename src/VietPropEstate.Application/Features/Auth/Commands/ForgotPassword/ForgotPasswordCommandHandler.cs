using MediatR;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IAuthService _authService;

    public ForgotPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        => _authService.ForgotPasswordAsync(request.Email, cancellationToken);
}
