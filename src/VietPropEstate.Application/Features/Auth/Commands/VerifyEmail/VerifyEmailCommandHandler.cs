using MediatR;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Auth.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
{
    private readonly IAuthService _authService;

    public VerifyEmailCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        => _authService.VerifyEmailAsync(request.UserId, request.Token, cancellationToken);
}
