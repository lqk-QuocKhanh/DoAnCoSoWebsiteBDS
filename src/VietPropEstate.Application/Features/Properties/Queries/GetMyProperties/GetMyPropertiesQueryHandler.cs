using AutoMapper;
using MediatR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Queries.GetMyProperties;

public sealed class GetMyPropertiesQueryHandler
    : IRequestHandler<GetMyPropertiesQuery, PaginatedList<PropertyDto>>
{
    private readonly IAgentService _agentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMyPropertiesQueryHandler(
        IAgentService agentService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _agentService = agentService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedList<PropertyDto>> Handle(
        GetMyPropertiesQuery request, CancellationToken cancellationToken)
    {
        var agentId = await _agentService.GetAgentIdForCurrentUserAsync(cancellationToken);
        if (!agentId.HasValue)
            return PaginatedList<PropertyDto>.Create([], 0, request.PageNumber, request.PageSize);

        var (items, total) = await _unitOfWork.Properties.GetByAgentPagedAsync(
            agentId.Value,
            request.PageNumber,
            request.PageSize,
            request.Status,
            cancellationToken);

        var dtos = _mapper.Map<List<PropertyDto>>(items);
        return PaginatedList<PropertyDto>.Create(dtos, total, request.PageNumber, request.PageSize);
    }
}
