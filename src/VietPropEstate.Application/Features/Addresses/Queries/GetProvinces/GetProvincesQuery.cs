using MediatR;
using VietPropEstate.Application.Features.Addresses.DTOs;

namespace VietPropEstate.Application.Features.Addresses.Queries.GetProvinces;

public sealed record GetProvincesQuery : IRequest<IReadOnlyList<ProvinceLookupDto>>;
