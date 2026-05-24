using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Constants;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Payments.DTOs;

namespace VietPropEstate.Application.Features.Payments.Queries.GetVipPackages;

public sealed class GetVipPackagesQueryHandler
    : IRequestHandler<GetVipPackagesQuery, IReadOnlyList<VIPPackageDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICacheService _cache;

    public GetVipPackagesQueryHandler(IApplicationDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public Task<IReadOnlyList<VIPPackageDto>> Handle(
        GetVipPackagesQuery request, CancellationToken cancellationToken)
        => _cache.GetOrSetAsync(
            CacheKeys.VipPackages,
            async ct => (IReadOnlyList<VIPPackageDto>)await _db.VIPPackages
                .AsNoTracking()
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Price.Amount)
                .Select(p => new VIPPackageDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price.Amount,
                    Currency = p.Price.Currency,
                    DurationDays = p.DurationDays,
                    MaxListings = p.MaxListings,
                    IsActive = p.IsActive
                })
                .ToListAsync(ct),
            CacheDurations.VipPackages,
            cancellationToken);
}
