using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.RemovePropertyImage;

public record RemovePropertyImageCommand(Guid PropertyId, Guid ImageId) : IRequest<Unit>;
