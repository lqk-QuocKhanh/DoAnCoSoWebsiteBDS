using MediatR;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.Queries.GetFavoriteProperties;

public record GetFavoritePropertiesQuery(string UserId, int PageNumber = 1, int PageSize = 12)
    : IRequest<PaginatedList<FavoritePropertyDto>>;
