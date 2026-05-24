using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

/// <summary>Represents a user's active subscription to a VIP package.</summary>
public class UserVIPPackage : AuditableEntity
{
    public string UserId { get; private set; } = string.Empty;
    public Guid VIPPackageId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int RemainingListings { get; private set; }
    public bool IsActive { get; private set; }

    public VIPPackage VIPPackage { get; private set; } = null!;

    private readonly List<Payment> _payments = [];
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private UserVIPPackage() { }

    public static UserVIPPackage Create(string userId, VIPPackage package, DateTime activationDate)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User ID is required.");

        var endDate = activationDate.AddDays(package.DurationDays);

        return new UserVIPPackage
        {
            UserId = userId,
            VIPPackageId = package.Id,
            StartDate = activationDate,
            EndDate = endDate,
            RemainingListings = package.MaxListings,
            IsActive = true
        };
    }

    public bool IsExpired => DateTime.UtcNow > EndDate;
    public bool HasListingsRemaining => RemainingListings > 0;

    public void UseOneListingSlot()
    {
        if (RemainingListings <= 0)
            throw new DomainException("No remaining listing slots in this VIP package.");

        RemainingListings--;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Extend(int additionalDays)
    {
        if (additionalDays <= 0)
            throw new DomainException("Extension days must be greater than zero.");

        EndDate = EndDate.AddDays(additionalDays);
    }
}
