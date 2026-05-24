using MediatR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterUserCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        => _authService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Role,
            request.IpAddress,
            cancellationToken);
}
