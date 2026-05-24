using MediatR;
using VietPropEstate.Application.Features.Payments.DTOs;

namespace VietPropEstate.Application.Features.Payments.Queries.GetUserVipPackages;

public sealed record GetUserVipPackagesQuery(string UserId, bool ActiveOnly = true)
    : IRequest<IReadOnlyList<UserVIPPackageDto>>;
