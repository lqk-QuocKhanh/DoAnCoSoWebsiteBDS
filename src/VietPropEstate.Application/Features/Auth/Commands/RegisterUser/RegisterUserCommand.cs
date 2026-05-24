using MediatR;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.RegisterUser;

public sealed record RegisterUserCommand : IRequest<AuthResponseDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }

    /// <summary>Requested role — defaults to Customer if omitted or invalid.</summary>
    public string Role { get; init; } = "Customer";

    /// <summary>Captured from HttpContext.Connection.RemoteIpAddress by the controller.</summary>
    public string? IpAddress { get; init; }
}
