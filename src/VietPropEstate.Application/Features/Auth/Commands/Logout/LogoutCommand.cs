using MediatR;

namespace VietPropEstate.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand : IRequest
{
    public string UserId { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}
