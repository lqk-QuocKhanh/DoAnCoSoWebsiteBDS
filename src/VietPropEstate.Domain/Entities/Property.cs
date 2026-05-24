using System.Text;
using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Events;
using VietPropEstate.Domain.Exceptions;
using VietPropEstate.Domain.ValueObjects;

namespace VietPropEstate.Domain.Entities;

public class Property : AuditableEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Money Price { get; private set; } = Money.Zero();
    public decimal Area { get; private set; }
    public int? NumberOfBedrooms { get; private set; }
    public int? NumberOfBathrooms { get; private set; }
    public int? NumberOfFloors { get; private set; }
    public PropertyStatus Status { get; private set; }
    public ListingType ListingType { get; private set; }
    public PropertyDirection? Direction { get; private set; }
    public Address Address { get; private set; } = null!;
    public int ViewCount { get; private set; }
    public bool IsFeatured { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? VideoUrl { get; private set; }

    public Guid PropertyTypeId { get; private set; }
    public PropertyType PropertyType { get; private set; } = null!;

    public Guid AgentId { get; private set; }
    public Agent Agent { get; private set; } = null!;

    public Guid? TransactionTypeId { get; private set; }
    public TransactionType? TransactionType { get; private set; }

    // --- Vietnam structured address (post-2025 administrative model) ---
    public int? ProvinceCode { get; private set; }
    public string? ProvinceName { get; private set; }
    public int? WardCode { get; private set; }
    public string? WardName { get; private set; }
    public string? FullAddress { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    public Province? Province { get; private set; }
    public Ward? Ward { get; private set; }

    private readonly List<PropertyImage> _images = [];
    public IReadOnlyCollection<PropertyImage> Images => _images.AsReadOnly();

    private readonly List<Transaction> _transactions = [];
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    private readonly List<Favorite> _favorites = [];
    public IReadOnlyCollection<Favorite> Favorites => _favorites.AsReadOnly();

    private readonly List<PropertyView> _propertyViews = [];
    public IReadOnlyCollection<PropertyView> PropertyViews => _propertyViews.AsReadOnly();

    private Property() { }

    public static Property Create(
        string title,
        string? description,
        Money price,
        decimal area,
        ListingType listingType,
        Address address,
        Guid propertyTypeId,
        Guid agentId,
        int? bedrooms = null,
        int? bathrooms = null,
        int? floors = null,
        PropertyDirection? direction = null,
        Guid? transactionTypeId = null,
        int? provinceCode = null,
        string? provinceName = null,
        int? wardCode = null,
        string? wardName = null,
        double? latitude = null,
        double? longitude = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Property title is required.");
        if (area <= 0)
            throw new DomainException("Property area must be greater than zero.");

        var property = new Property
        {
            Title = title,
            Description = description,
            Price = price,
            Area = area,
            ListingType = listingType,
            Address = address,
            PropertyTypeId = propertyTypeId,
            AgentId = agentId,
            NumberOfBedrooms = bedrooms,
            NumberOfBathrooms = bathrooms,
            NumberOfFloors = floors,
            Direction = direction,
            Status = PropertyStatus.Draft,
            ViewCount = 0,
            IsFeatured = false,
            TransactionTypeId = transactionTypeId,
            ProvinceCode = provinceCode,
            ProvinceName = provinceName,
            WardCode = wardCode,
            WardName = wardName,
            Latitude = latitude,
            Longitude = longitude
        };

        property.RefreshFullAddress();
        property.Slug = BuildSlug(title, property.Id);

        property.AddDomainEvent(new PropertyCreatedEvent(property.Id));
        return property;
    }

    public void UpdateDetails(
        string title,
        string? description,
        Money price,
        decimal area,
        int? bedrooms,
        int? bathrooms,
        int? floors,
        PropertyDirection? direction,
        string? videoUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Property title is required.");
        if (area <= 0)
            throw new DomainException("Property area must be greater than zero.");

        Title = title;
        Slug = BuildSlug(title, Id);
        Description = description;
        Price = price;
        Area = area;
        NumberOfBedrooms = bedrooms;
        NumberOfBathrooms = bathrooms;
        NumberOfFloors = floors;
        Direction = direction;
        VideoUrl = videoUrl;

        AddDomainEvent(new PropertyUpdatedEvent(Id));
    }

    public void SubmitForApproval()
    {
        if (Status is not (PropertyStatus.Draft or PropertyStatus.Rejected or PropertyStatus.Withdrawn))
            throw new DomainException("Property cannot be submitted for approval in its current state.");

        Status = PropertyStatus.PendingApproval;
    }

    public void Publish()
    {
        if (Status != PropertyStatus.PendingApproval)
            throw new DomainException("Only pending approval properties can be published.");

        Status = PropertyStatus.Active;
        PublishedAt = DateTime.UtcNow;
        AddDomainEvent(new PropertyPublishedEvent(Id));
    }

    public void Reject()
    {
        if (Status != PropertyStatus.PendingApproval)
            throw new DomainException("Only pending approval properties can be rejected.");

        Status = PropertyStatus.Rejected;
    }

    public void Withdraw()
    {
        if (Status == PropertyStatus.Sold || Status == PropertyStatus.Rented)
            throw new DomainException("Sold or rented properties cannot be withdrawn.");

        Status = PropertyStatus.Withdrawn;
    }

    public void RestoreFromHidden()
    {
        if (Status != PropertyStatus.Withdrawn)
            throw new DomainException("Only hidden properties can be restored.");

        Status = PropertyStatus.Active;
        AddDomainEvent(new PropertyPublishedEvent(Id));
    }

    public void MarkAsSold()
    {
        if (Status != PropertyStatus.Active && Status != PropertyStatus.UnderOffer)
            throw new DomainException("Only active or under-offer properties can be marked as sold.");

        Status = PropertyStatus.Sold;
        AddDomainEvent(new PropertyStatusChangedEvent(Id, PropertyStatus.Sold));
    }

    public void MarkAsRented()
    {
        if (Status != PropertyStatus.Active && Status != PropertyStatus.UnderOffer)
            throw new DomainException("Only active or under-offer properties can be marked as rented.");

        Status = PropertyStatus.Rented;
        AddDomainEvent(new PropertyStatusChangedEvent(Id, PropertyStatus.Rented));
    }

    public void PlaceUnderOffer()
    {
        if (Status != PropertyStatus.Active)
            throw new DomainException("Only active properties can be placed under offer.");

        Status = PropertyStatus.UnderOffer;
    }

    public void SetFeatured(bool featured) => IsFeatured = featured;

    public void SetExpiry(DateTime expiresAt) => ExpiresAt = expiresAt;

    public void IncrementViewCount() => ViewCount++;

    public void AddImage(PropertyImage image)
    {
        if (image.IsPrimary)
        {
            foreach (var existing in _images)
                existing.ClearPrimary();
        }

        _images.Add(image);
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new DomainException($"Image {imageId} not found on this property.");
        _images.Remove(image);
    }

    public void UpdateAddress(Address address) => Address = address;

    public void SetTransactionType(Guid? transactionTypeId) => TransactionTypeId = transactionTypeId;

    public void UpdateVietnamAddress(
        int? provinceCode,
        string? provinceName,
        int? wardCode,
        string? wardName,
        double? latitude,
        double? longitude)
    {
        ProvinceCode = provinceCode;
        ProvinceName = provinceName;
        WardCode = wardCode;
        WardName = wardName;
        Latitude = latitude;
        Longitude = longitude;
        RefreshFullAddress();
    }

    private static string BuildSlug(string title, Guid id)
    {
        var sb = new StringBuilder();
        foreach (var c in title.ToLowerInvariant())
        {
            if (c == ' ' || c == '-') sb.Append('-');
            else if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) sb.Append(c);
        }

        var slug = sb.ToString().Trim('-');
        while (slug.Contains("--")) slug = slug.Replace("--", "-");

        return $"{slug}-{id.ToString("N")[..8]}";
    }

    private void RefreshFullAddress()
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(Address?.Street)) parts.Add(Address.Street);
        if (!string.IsNullOrWhiteSpace(WardName)) parts.Add(WardName);
        if (!string.IsNullOrWhiteSpace(ProvinceName)) parts.Add(ProvinceName);
        FullAddress = parts.Count > 0 ? string.Join(", ", parts) : null;
    }
}
