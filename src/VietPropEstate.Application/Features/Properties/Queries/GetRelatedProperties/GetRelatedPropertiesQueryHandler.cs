using AutoMapper;
using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Queries.GetRelatedProperties;

public sealed class GetRelatedPropertiesQueryHandler
    : IRequestHandler<GetRelatedPropertiesQuery, IReadOnlyList<PropertyDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRelatedPropertiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PropertyDto>> Handle(
        GetRelatedPropertiesQuery request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.PropertyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.PropertyId);

        var count = Math.Clamp(request.Count, 1, 20);
        var related = await _unitOfWork.Properties.GetRelatedAsync(
            request.PropertyId, property.PropertyTypeId, property.ProvinceCode, count, cancellationToken);

        return _mapper.Map<List<PropertyDto>>(related);
    }
}
