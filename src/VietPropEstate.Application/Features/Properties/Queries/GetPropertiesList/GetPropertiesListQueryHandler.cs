using AutoMapper;
using MediatR;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPropertiesList;

public sealed class GetPropertiesListQueryHandler
    : IRequestHandler<GetPropertiesListQuery, PaginatedList<PropertyDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPropertiesListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedList<PropertyDto>> Handle(
        GetPropertiesListQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Properties.GetPagedAsync(
            request.PageNumber, request.PageSize,
            request.SearchTerm,
            request.PropertyTypeId, request.TransactionTypeId,
            request.ListingType, request.Status,
            request.ProvinceCode, request.WardCode, request.ProvinceName,
            request.MinPrice, request.MaxPrice,
            request.MinArea, request.MaxArea,
            request.MinBedrooms, request.MaxBedrooms,
            request.Bathrooms, request.Direction,
            request.IsFeatured,
            request.AllStatuses,
            request.SortBy, request.SortOrder,
            cancellationToken);

        var dtos = _mapper.Map<List<PropertyDto>>(items);
        return PaginatedList<PropertyDto>.Create(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
