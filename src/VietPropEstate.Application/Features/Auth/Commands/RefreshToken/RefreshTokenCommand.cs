using MediatR;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand : IRequest<AuthResponseDto>
{
    public string RefreshToken { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}
