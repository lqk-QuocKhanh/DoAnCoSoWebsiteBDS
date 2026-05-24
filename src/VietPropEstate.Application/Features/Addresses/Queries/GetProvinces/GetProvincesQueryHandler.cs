using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Constants;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Addresses.DTOs;

namespace VietPropEstate.Application.Features.Addresses.Queries.GetProvinces;

public sealed class GetProvincesQueryHandler
    : IRequestHandler<GetProvincesQuery, IReadOnlyList<ProvinceLookupDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICacheService _cache;

    public GetProvincesQueryHandler(IApplicationDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public Task<IReadOnlyList<ProvinceLookupDto>> Handle(
        GetProvincesQuery request, CancellationToken cancellationToken)
        => _cache.GetOrSetAsync(
            CacheKeys.Provinces,
            async ct => (IReadOnlyList<ProvinceLookupDto>)await _db.Provinces
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .Select(p => new ProvinceLookupDto
                {
                    Code = p.Code,
                    Name = p.Name,
                    Codename = p.Codename,
                    DivisionType = p.DivisionType
                })
                .ToListAsync(ct),
            CacheDurations.AddressData,
            cancellationToken);
}
