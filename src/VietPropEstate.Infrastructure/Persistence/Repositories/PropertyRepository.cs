using Microsoft.EntityFrameworkCore;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Infrastructure.Persistence.Repositories;

public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
{
    public PropertyRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Property?> GetByIdWithDetailsAsync(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyDetailIncludes(_dbSet);
        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Property?> GetBySlugAsync(
        string slug, CancellationToken cancellationToken = default)
        => await ApplyDetailIncludes(_dbSet)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

    public async Task<(IReadOnlyList<Property> Items, int TotalCount)> GetPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = ApplyListIncludes(_dbSet).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(term) ||
                (p.Description != null && p.Description.ToLower().Contains(term)) ||
                (p.FullAddress != null && p.FullAddress.ToLower().Contains(term)));
        }

        if (propertyTypeId.HasValue)
            query = query.Where(p => p.PropertyTypeId == propertyTypeId.Value);

        if (transactionTypeId.HasValue)
            query = query.Where(p => p.TransactionTypeId == transactionTypeId.Value);

        if (listingType.HasValue)
            query = query.Where(p => p.ListingType == listingType.Value);

        query = status.HasValue
            ? query.Where(p => p.Status == status.Value)
            : allStatuses
                ? query
                : query.Where(p => p.Status == PropertyStatus.Active);

        if (provinceCode.HasValue)
            query = query.Where(p => p.ProvinceCode == provinceCode.Value);
        else if (!string.IsNullOrWhiteSpace(provinceName))
            query = query.Where(p => p.ProvinceName != null &&
                                     p.ProvinceName.ToLower().Contains(provinceName.ToLower()));

        if (wardCode.HasValue)
            query = query.Where(p => p.WardCode == wardCode.Value);

        if (minPrice.HasValue)
            query = query.Where(p => EF.Property<decimal>(p, "PriceAmount") >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => EF.Property<decimal>(p, "PriceAmount") <= maxPrice.Value);

        if (minArea.HasValue)
            query = query.Where(p => p.Area >= minArea.Value);

        if (maxArea.HasValue)
            query = query.Where(p => p.Area <= maxArea.Value);

        if (minBedrooms.HasValue)
            query = query.Where(p => p.NumberOfBedrooms >= minBedrooms.Value);

        if (maxBedrooms.HasValue)
            query = query.Where(p => p.NumberOfBedrooms <= maxBedrooms.Value);

        if (bathrooms.HasValue)
            query = query.Where(p => p.NumberOfBathrooms == bathrooms.Value);

        if (direction.HasValue)
            query = query.Where(p => p.Direction == direction.Value);

        if (isFeatured.HasValue)
            query = query.Where(p => p.IsFeatured == isFeatured.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        bool desc = sortOrder.Equals("Desc", StringComparison.OrdinalIgnoreCase);
        query = (sortBy.ToLower(), desc) switch
        {
            ("price",  true)  => query.OrderByDescending(p => EF.Property<decimal>(p, "PriceAmount")),
            ("price",  false) => query.OrderBy(p => EF.Property<decimal>(p, "PriceAmount")),
            ("area",   true)  => query.OrderByDescending(p => p.Area),
            ("area",   false) => query.OrderBy(p => p.Area),
            ("views",  true)  => query.OrderByDescending(p => p.ViewCount),
            ("views",  false) => query.OrderBy(p => p.ViewCount),
            ("published", true)  => query.OrderByDescending(p => p.PublishedAt),
            ("published", false) => query.OrderBy(p => p.PublishedAt),
            _  => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt)
        };

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Property>> GetFeaturedAsync(
        int count, CancellationToken cancellationToken = default)
        => await ApplyListIncludes(_dbSet)
            .AsNoTracking()
            .Where(p => p.IsFeatured && p.Status == PropertyStatus.Active)
            .OrderByDescending(p => p.PublishedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Property>> GetByAgentAsync(
        Guid agentId, CancellationToken cancellationToken = default)
        => await ApplyListIncludes(_dbSet)
            .AsNoTracking()
            .Where(p => p.AgentId == agentId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<(IReadOnlyList<Property> Items, int TotalCount)> GetByAgentPagedAsync(
        Guid agentId,
        int pageNumber,
        int pageSize,
        PropertyStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyListIncludes(_dbSet)
            .AsNoTracking()
            .Where(p => p.AgentId == agentId);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<Property>> GetRelatedAsync(
        Guid propertyId,
        Guid propertyTypeId,
        int? provinceCode,
        int count,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyListIncludes(_dbSet)
            .AsNoTracking()
            .Where(p => p.Id != propertyId && p.Status == PropertyStatus.Active);

        if (provinceCode.HasValue)
        {
            var withProvince = await query
                .Where(p => p.PropertyTypeId == propertyTypeId && p.ProvinceCode == provinceCode.Value)
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);

            if (withProvince.Count >= count) return withProvince;

            var existing = withProvince.Select(p => p.Id).ToHashSet();
            var topUp = await query
                .Where(p => p.PropertyTypeId == propertyTypeId && !existing.Contains(p.Id))
                .OrderByDescending(p => p.CreatedAt)
                .Take(count - withProvince.Count)
                .ToListAsync(cancellationToken);

            return [.. withProvince, .. topUp];
        }

        return await query
            .Where(p => p.PropertyTypeId == propertyTypeId)
            .OrderByDescending(p => p.IsFeatured)
            .ThenByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Property>> GetRecentlyViewedAsync(
        string userId,
        int count,
        CancellationToken cancellationToken = default)
    {
        var recentIds = await _context.Set<PropertyView>()
            .AsNoTracking()
            .Where(pv => pv.UserId == userId)
            .OrderByDescending(pv => pv.ViewedAt)
            .Select(pv => pv.PropertyId)
            .Distinct()
            .Take(count)
            .ToListAsync(cancellationToken);

        if (recentIds.Count == 0) return [];

        var properties = await ApplyListIncludes(_dbSet)
            .AsNoTracking()
            .Where(p => recentIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        return recentIds
            .Select(id => properties.FirstOrDefault(p => p.Id == id))
            .OfType<Property>()
            .ToList();
    }

    public async Task IncrementViewCountAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        if (!_context.Database.IsRelational())
        {
            var property = await _dbSet.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (property is null)
                return;

            property.IncrementViewCount();
            return;
        }

        await _dbSet
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1),
                cancellationToken);
    }

    public async Task AddPropertyImageAsync(
        PropertyImage image, CancellationToken cancellationToken = default)
    {
        if (image.IsPrimary)
        {
            if (_context.Database.IsRelational())
            {
                await _context.PropertyImages
                    .Where(pi => pi.PropertyId == image.PropertyId && pi.IsPrimary)
                    .ExecuteUpdateAsync(
                        s => s.SetProperty(pi => pi.IsPrimary, false),
                        cancellationToken);
            }
            else
            {
                var existingPrimaryImages = await _context.PropertyImages
                    .Where(pi => pi.PropertyId == image.PropertyId && pi.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var existing in existingPrimaryImages)
                    existing.ClearPrimary();
            }
        }

        await _context.PropertyImages.AddAsync(image, cancellationToken);
    }

    private static IQueryable<Property> ApplyDetailIncludes(IQueryable<Property> query)
        => query
            .Include(p => p.PropertyType)
            .Include(p => p.Agent)
            .Include(p => p.TransactionType)
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder));

    private static IQueryable<Property> ApplyListIncludes(IQueryable<Property> query)
        => query
            .Include(p => p.PropertyType)
            .Include(p => p.Agent)
            .Include(p => p.TransactionType)
            .Include(p => p.Images);
}
