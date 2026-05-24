using MediatR;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.LoginUser;

public sealed record LoginUserCommand : IRequest<AuthResponseDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}
