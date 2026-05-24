using MediatR;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Auth.Commands.RevokeToken;

public sealed class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IAuthService _authService;

    public RevokeTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        => _authService.RevokeTokenAsync(request.RefreshToken, request.IpAddress, request.Reason, cancellationToken);
}
