using MediatR;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPropertyById;

public record GetPropertyByIdQuery(Guid Id) : IRequest<PropertyDetailDto>;
