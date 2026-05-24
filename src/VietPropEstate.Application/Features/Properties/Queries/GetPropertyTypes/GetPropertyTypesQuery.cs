using MediatR;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPropertyTypes;

public record GetPropertyTypesQuery : IRequest<IReadOnlyList<PropertyTypeDto>>;
