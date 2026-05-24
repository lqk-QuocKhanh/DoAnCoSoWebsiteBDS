namespace VietPropEstate.Application.Features.Auth.DTOs;

public sealed record UserProfileDto
{
    public string UserId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? AvatarUrl { get; init; }
    public string? AddressLine { get; init; }
    public string? ProvinceName { get; init; }
    public string? WardName { get; init; }
    public int? ProvinceCode { get; init; }
    public int? WardCode { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool PhoneNumberConfirmed { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public IList<string> Roles { get; init; } = [];
}
