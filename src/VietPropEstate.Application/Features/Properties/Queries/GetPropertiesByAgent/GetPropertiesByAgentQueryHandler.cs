using AutoMapper;
using MediatR;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPropertiesByAgent;

public sealed class GetPropertiesByAgentQueryHandler
    : IRequestHandler<GetPropertiesByAgentQuery, PaginatedList<PropertyDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPropertiesByAgentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedList<PropertyDto>> Handle(
        GetPropertiesByAgentQuery request, CancellationToken cancellationToken)
    {
        var all = await _unitOfWork.Properties.GetByAgentAsync(request.AgentId, cancellationToken);
        var page = all.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
        var dtos = _mapper.Map<List<PropertyDto>>(page);
        return PaginatedList<PropertyDto>.Create(dtos, all.Count, request.PageNumber, request.PageSize);
    }
}
