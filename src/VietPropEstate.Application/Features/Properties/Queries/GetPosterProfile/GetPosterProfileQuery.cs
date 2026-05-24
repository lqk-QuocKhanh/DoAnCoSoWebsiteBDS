using MediatR;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPosterProfile;

public sealed record GetPosterProfileQuery(string UserId, int PageNumber = 1, int PageSize = 12)
    : IRequest<PosterProfileDto?>;
