using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Domain.Interfaces;

public interface IPropertyRepository : IRepository<Property>
{
    Task<Property?> GetByIdWithDetailsAsync(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    Task<Property?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Property> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? propertyTypeId = null,
        Guid? transactionTypeId = null,
        ListingType? listingType = null,
        PropertyStatus? status = null,
        int? provinceCode = null,
        int? wardCode = null,
        string? provinceName = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        decimal? minArea = null,
        decimal? maxArea = null,
        int? minBedrooms = null,
        int? maxBedrooms = null,
        int? bathrooms = null,
        PropertyDirection? direction = null,
        bool? isFeatured = null,
        bool allStatuses = false,
        string sortBy = "CreatedAt",
        string sortOrder = "Desc",
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Property>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Property>> GetByAgentAsync(Guid agentId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Property> Items, int TotalCount)> GetByAgentPagedAsync(
        Guid agentId,
        int pageNumber,
        int pageSize,
        PropertyStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Property>> GetRelatedAsync(
        Guid propertyId,
        Guid propertyTypeId,
        int? provinceCode,
        int count,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Property>> GetRecentlyViewedAsync(
        string userId,
        int count,
        CancellationToken cancellationToken = default);

    Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddPropertyImageAsync(PropertyImage image, CancellationToken cancellationToken = default);
}
