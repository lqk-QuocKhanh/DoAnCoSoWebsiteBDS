using MediatR;

namespace VietPropEstate.Application.Features.Auth.Commands.RevokeToken;

public sealed record RevokeTokenCommand : IRequest
{
    public string RefreshToken { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
    public string? Reason { get; init; }
}
