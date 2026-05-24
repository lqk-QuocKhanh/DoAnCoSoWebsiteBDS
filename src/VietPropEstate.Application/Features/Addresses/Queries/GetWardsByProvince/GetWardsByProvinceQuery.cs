using MediatR;
using VietPropEstate.Application.Features.Addresses.DTOs;

namespace VietPropEstate.Application.Features.Addresses.Queries.GetWardsByProvince;

public sealed record GetWardsByProvinceQuery(int ProvinceCode)
    : IRequest<IReadOnlyList<WardLookupDto>>;
