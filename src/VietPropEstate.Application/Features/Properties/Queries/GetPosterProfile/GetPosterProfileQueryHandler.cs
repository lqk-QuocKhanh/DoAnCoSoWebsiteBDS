using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPosterProfile;

public sealed class GetPosterProfileQueryHandler : IRequestHandler<GetPosterProfileQuery, PosterProfileDto?>
{
    private static readonly PropertyStatus[] PublicStatuses =
    [
        PropertyStatus.Active,
        PropertyStatus.UnderOffer,
        PropertyStatus.Sold,
        PropertyStatus.Rented
    ];

    private readonly IApplicationDbContext _db;
    private readonly IUserPublicProfileService _profiles;
    private readonly IMapper _mapper;

    public GetPosterProfileQueryHandler(
        IApplicationDbContext db,
        IUserPublicProfileService profiles,
        IMapper mapper)
    {
        _db = db;
        _profiles = profiles;
        _mapper = mapper;
    }

    public async Task<PosterProfileDto?> Handle(
        GetPosterProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _profiles.GetProfileAsync(request.UserId, cancellationToken);
        if (profile is null)
            return null;

        var totalCount = 0;
        var listingDtos = new List<PropertyDto>();

        if (profile.AgentId.HasValue)
        {
            var listingsQuery = _db.Properties
                .AsNoTracking()
                .Include(p => p.PropertyType)
                .Include(p => p.TransactionType)
                .Include(p => p.Images.Where(i => i.IsPrimary))
                .Where(p => p.AgentId == profile.AgentId.Value && !p.IsDeleted && PublicStatuses.Contains(p.Status));

            totalCount = await listingsQuery.CountAsync(cancellationToken);

            var listings = await listingsQuery
                .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            listingDtos = _mapper.Map<List<PropertyDto>>(listings);
        }

        return new PosterProfileDto
        {
            UserId = profile.UserId,
            AgentId = profile.AgentId ?? Guid.Empty,
            DisplayName = profile.DisplayName,
            PhoneNumber = profile.PhoneNumber,
            Email = profile.Email,
            AvatarUrl = profile.AvatarUrl,
            AgencyName = profile.AgencyName,
            Bio = profile.Bio,
            MemberSince = profile.MemberSince,
            TotalListings = totalCount,
            Listings = PaginatedList<PropertyDto>.Create(
                listingDtos, totalCount, request.PageNumber, request.PageSize)
        };
    }
}
