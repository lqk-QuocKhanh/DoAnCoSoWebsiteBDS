using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.RejectProperty;

public record RejectPropertyCommand(Guid Id) : IRequest<Unit>;
