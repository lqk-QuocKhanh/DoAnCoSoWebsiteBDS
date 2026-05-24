using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.PublishProperty;

public record PublishPropertyCommand(Guid Id) : IRequest<Unit>;
