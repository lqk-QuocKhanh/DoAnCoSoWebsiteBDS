using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Constants;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Addresses.DTOs;

namespace VietPropEstate.Application.Features.Addresses.Queries.GetWardsByProvince;

public sealed class GetWardsByProvinceQueryHandler
    : IRequestHandler<GetWardsByProvinceQuery, IReadOnlyList<WardLookupDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICacheService _cache;

    public GetWardsByProvinceQueryHandler(IApplicationDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public Task<IReadOnlyList<WardLookupDto>> Handle(
        GetWardsByProvinceQuery request, CancellationToken cancellationToken)
        => _cache.GetOrSetAsync(
            CacheKeys.Wards(request.ProvinceCode),
            async ct => (IReadOnlyList<WardLookupDto>)await _db.Wards
                .AsNoTracking()
                .Where(w => w.IsActive && w.ProvinceCode == request.ProvinceCode)
                .OrderBy(w => w.Name)
                .Select(w => new WardLookupDto
                {
                    Code = w.Code,
                    Name = w.Name,
                    Codename = w.Codename,
                    DivisionType = w.DivisionType,
                    ProvinceCode = w.ProvinceCode
                })
                .ToListAsync(ct),
            CacheDurations.AddressData,
            cancellationToken);
}
