using MediatR;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.Queries.GetFeaturedProperties;

public record GetFeaturedPropertiesQuery(int Count = 8) : IRequest<IReadOnlyList<PropertyDto>>;
