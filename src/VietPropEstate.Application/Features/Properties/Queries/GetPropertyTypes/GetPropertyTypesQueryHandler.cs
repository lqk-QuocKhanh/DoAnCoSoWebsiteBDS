using AutoMapper;
using MediatR;
using VietPropEstate.Application.Common.Constants;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPropertyTypes;

public sealed class GetPropertyTypesQueryHandler
    : IRequestHandler<GetPropertyTypesQuery, IReadOnlyList<PropertyTypeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public GetPropertyTypesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public Task<IReadOnlyList<PropertyTypeDto>> Handle(
        GetPropertyTypesQuery request, CancellationToken cancellationToken)
        => _cache.GetOrSetAsync(
            CacheKeys.PropertyTypes,
            async ct =>
            {
                var types = await _unitOfWork.PropertyTypes.FindAsync(
                    pt => pt.IsActive && !pt.IsDeleted, ct);
                return (IReadOnlyList<PropertyTypeDto>)_mapper.Map<List<PropertyTypeDto>>(types);
            },
            CacheDurations.LookupData,
            cancellationToken);
}
