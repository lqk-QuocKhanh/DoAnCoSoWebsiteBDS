using MediatR;
using VietPropEstate.Application.Features.Payments.DTOs;

namespace VietPropEstate.Application.Features.Payments.Queries.GetVipPackages;

public sealed record GetVipPackagesQuery : IRequest<IReadOnlyList<VIPPackageDto>>;
