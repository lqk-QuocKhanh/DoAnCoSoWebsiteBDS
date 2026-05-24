using MediatR;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPropertiesByAgent;

public record GetPropertiesByAgentQuery(Guid AgentId, int PageNumber = 1, int PageSize = 12)
    : IRequest<PaginatedList<PropertyDto>>;
