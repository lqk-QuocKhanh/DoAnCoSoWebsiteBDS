using AutoMapper;
using MediatR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;
using Microsoft.EntityFrameworkCore;

namespace VietPropEstate.Application.Features.Properties.Queries.GetFavoriteProperties;

public sealed class GetFavoritePropertiesQueryHandler
    : IRequestHandler<GetFavoritePropertiesQuery, PaginatedList<FavoritePropertyDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFavoritePropertiesQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedList<FavoritePropertyDto>> Handle(
        GetFavoritePropertiesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Favorites
            .Where(f => f.UserId == request.UserId && !f.IsDeleted)
            .Include(f => f.Property)
                .ThenInclude(p => p.PropertyType)
            .Include(f => f.Property)
                .ThenInclude(p => p.Images.Where(i => i.IsPrimary).Take(1))
            .OrderByDescending(f => f.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var favorites = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = favorites.Select(f => new FavoritePropertyDto
        {
            FavoriteId = f.Id,
            Note = f.Note,
            FavoritedAt = f.CreatedAt,
            Property = _mapper.Map<PropertyDto>(f.Property)
        }).ToList();

        return PaginatedList<FavoritePropertyDto>.Create(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
