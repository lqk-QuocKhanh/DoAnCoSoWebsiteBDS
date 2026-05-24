using MediatR;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Properties.Queries.GetMyProperties;

public record GetMyPropertiesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    PropertyStatus? Status = null) : IRequest<PaginatedList<PropertyDto>>;
