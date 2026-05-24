using AutoMapper;
using MediatR;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Queries.GetFeaturedProperties;

public sealed class GetFeaturedPropertiesQueryHandler
    : IRequestHandler<GetFeaturedPropertiesQuery, IReadOnlyList<PropertyDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetFeaturedPropertiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PropertyDto>> Handle(
        GetFeaturedPropertiesQuery request, CancellationToken cancellationToken)
    {
        var count = Math.Clamp(request.Count, 1, 50);
        var properties = await _unitOfWork.Properties.GetFeaturedAsync(count, cancellationToken);
        return _mapper.Map<List<PropertyDto>>(properties);
    }
}
