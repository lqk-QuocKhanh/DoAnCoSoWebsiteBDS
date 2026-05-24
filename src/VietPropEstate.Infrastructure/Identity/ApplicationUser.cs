using Microsoft.AspNetCore.Identity;

namespace VietPropEstate.Infrastructure.Identity;

/// <summary>
/// Extended Identity user with domain profile fields.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? AddressLine { get; set; }
    public string? ProvinceName { get; set; }
    public string? WardName { get; set; }
    public int? ProvinceCode { get; set; }
    public int? WardCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    public string FullName =>
        string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
            ? Email ?? string.Empty
            : $"{FirstName} {LastName}".Trim();
}
