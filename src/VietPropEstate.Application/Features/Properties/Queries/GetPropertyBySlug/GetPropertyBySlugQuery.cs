using MediatR;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPropertyBySlug;

public record GetPropertyBySlugQuery(string Slug) : IRequest<PropertyDetailDto>;
