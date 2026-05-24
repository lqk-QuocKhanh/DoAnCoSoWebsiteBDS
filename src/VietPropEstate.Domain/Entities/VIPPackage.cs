using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;
using VietPropEstate.Domain.ValueObjects;

namespace VietPropEstate.Domain.Entities;

/// <summary>A VIP advertising package that agents / owners can purchase.</summary>
public class VIPPackage : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    /// <summary>How long the package is valid after activation (in days).</summary>
    public int DurationDays { get; private set; }

    public Money Price { get; private set; } = Money.Zero();

    /// <summary>Maximum number of property listings allowed under this package.</summary>
    public int MaxListings { get; private set; }

    /// <summary>Whether new subscriptions can be purchased.</summary>
    public bool IsActive { get; private set; } = true;

    private readonly List<UserVIPPackage> _userPackages = [];
    public IReadOnlyCollection<UserVIPPackage> UserPackages => _userPackages.AsReadOnly();

    private VIPPackage() { }

    public static VIPPackage Create(
        string name,
        Money price,
        int durationDays,
        int maxListings,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("VIP package name is required.");
        if (durationDays <= 0)
            throw new DomainException("Duration must be greater than zero days.");
        if (maxListings <= 0)
            throw new DomainException("Max listings must be greater than zero.");

        return new VIPPackage
        {
            Name = name.Trim(),
            Price = price,
            DurationDays = durationDays,
            MaxListings = maxListings,
            Description = description,
            IsActive = true
        };
    }

    public void Update(string name, Money price, int durationDays, int maxListings, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("VIP package name is required.");
        if (durationDays <= 0)
            throw new DomainException("Duration must be greater than zero days.");
        if (maxListings <= 0)
            throw new DomainException("Max listings must be greater than zero.");

        Name = name.Trim();
        Price = price;
        DurationDays = durationDays;
        MaxListings = maxListings;
        Description = description;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
