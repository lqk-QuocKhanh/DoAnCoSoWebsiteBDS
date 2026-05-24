using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

public class Agent : AuditableEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string? LicenseNumber { get; private set; }
    public string? AgencyName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? Bio { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? UserId { get; private set; }

    private readonly List<Property> _properties = [];
    public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();

    private Agent() { }

    public static Agent Create(
        string fullName,
        string email,
        string phoneNumber,
        string? licenseNumber = null,
        string? agencyName = null,
        string? userId = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Agent full name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Agent email is required.");

        return new Agent
        {
            FullName = fullName,
            Email = email,
            PhoneNumber = phoneNumber,
            LicenseNumber = licenseNumber,
            AgencyName = agencyName,
            UserId = userId,
            IsActive = true
        };
    }

    public void UpdateProfile(string fullName, string phoneNumber, string? agencyName, string? bio, string? avatarUrl)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        AgencyName = agencyName;
        Bio = bio;
        AvatarUrl = avatarUrl;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
