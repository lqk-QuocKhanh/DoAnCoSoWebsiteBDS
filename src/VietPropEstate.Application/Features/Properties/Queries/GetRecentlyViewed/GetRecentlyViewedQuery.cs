using MediatR;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.Queries.GetRecentlyViewed;

public record GetRecentlyViewedQuery(string UserId, int Count = 10) : IRequest<IReadOnlyList<PropertyDto>>;
