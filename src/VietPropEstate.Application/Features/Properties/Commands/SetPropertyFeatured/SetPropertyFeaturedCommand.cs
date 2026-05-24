using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.SetPropertyFeatured;

public record SetPropertyFeaturedCommand(Guid Id, bool IsFeatured) : IRequest<Unit>;
