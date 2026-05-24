using MediatR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.LoginUser;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public LoginUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        => _authService.LoginAsync(request.Email, request.Password, request.IpAddress, cancellationToken);
}
