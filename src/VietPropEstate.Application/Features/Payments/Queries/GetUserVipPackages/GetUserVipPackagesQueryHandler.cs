using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Payments.DTOs;

namespace VietPropEstate.Application.Features.Payments.Queries.GetUserVipPackages;

public sealed class GetUserVipPackagesQueryHandler
    : IRequestHandler<GetUserVipPackagesQuery, IReadOnlyList<UserVIPPackageDto>>
{
    private readonly IApplicationDbContext _db;

    public GetUserVipPackagesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<UserVIPPackageDto>> Handle(
        GetUserVipPackagesQuery request, CancellationToken cancellationToken)
    {
        var query = _db.UserVIPPackages
            .Include(u => u.VIPPackage)
            .Where(u => u.UserId == request.UserId && !u.IsDeleted);

        if (request.ActiveOnly)
            query = query.Where(u => u.IsActive && u.EndDate > DateTime.UtcNow);

        var items = await query
            .OrderByDescending(u => u.StartDate)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        return items.Select(u => new UserVIPPackageDto
        {
            Id = u.Id,
            UserId = u.UserId,
            VIPPackageId = u.VIPPackageId,
            VIPPackageName = u.VIPPackage?.Name,
            StartDate = u.StartDate,
            EndDate = u.EndDate,
            RemainingListings = u.RemainingListings,
            IsActive = u.IsActive,
            IsExpired = u.IsExpired,
            DaysRemaining = Math.Max(0, (int)(u.EndDate - now).TotalDays)
        }).ToList();
    }
}
