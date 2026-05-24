using AutoMapper;
using MediatR;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Queries.GetRecentlyViewed;

public sealed class GetRecentlyViewedQueryHandler
    : IRequestHandler<GetRecentlyViewedQuery, IReadOnlyList<PropertyDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRecentlyViewedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PropertyDto>> Handle(
        GetRecentlyViewedQuery request, CancellationToken cancellationToken)
    {
        var count = Math.Clamp(request.Count, 1, 50);
        var properties = await _unitOfWork.Properties
            .GetRecentlyViewedAsync(request.UserId, count, cancellationToken);

        return _mapper.Map<List<PropertyDto>>(properties);
    }
}
