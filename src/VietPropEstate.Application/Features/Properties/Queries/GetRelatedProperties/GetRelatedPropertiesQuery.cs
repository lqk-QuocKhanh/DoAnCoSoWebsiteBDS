using MediatR;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.Queries.GetRelatedProperties;

public record GetRelatedPropertiesQuery(Guid PropertyId, int Count = 6) : IRequest<IReadOnlyList<PropertyDto>>;
