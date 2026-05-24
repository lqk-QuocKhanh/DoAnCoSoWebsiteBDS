namespace VietPropEstate.Application.Features.Auth.DTOs;

public sealed record AuthResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public bool EmailConfirmed { get; init; }
    public IList<string> Roles { get; init; } = [];
}
