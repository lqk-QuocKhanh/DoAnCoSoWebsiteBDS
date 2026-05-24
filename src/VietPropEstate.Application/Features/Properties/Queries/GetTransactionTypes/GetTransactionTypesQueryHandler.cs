using AutoMapper;
using MediatR;
using VietPropEstate.Application.Common.Constants;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Queries.GetTransactionTypes;

public sealed class GetTransactionTypesQueryHandler
    : IRequestHandler<GetTransactionTypesQuery, IReadOnlyList<TransactionTypeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public GetTransactionTypesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public Task<IReadOnlyList<TransactionTypeDto>> Handle(
        GetTransactionTypesQuery request, CancellationToken cancellationToken)
        => _cache.GetOrSetAsync(
            CacheKeys.TransactionTypes,
            async ct =>
            {
                var types = await _unitOfWork.TransactionTypes.FindAsync(
                    tt => tt.IsActive && !tt.IsDeleted, ct);
                return (IReadOnlyList<TransactionTypeDto>)_mapper.Map<List<TransactionTypeDto>>(types);
            },
            CacheDurations.LookupData,
            cancellationToken);
}
