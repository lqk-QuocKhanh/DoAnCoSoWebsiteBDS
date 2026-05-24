using MediatR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        => _authService.RefreshTokenAsync(request.RefreshToken, request.IpAddress, cancellationToken);
}
