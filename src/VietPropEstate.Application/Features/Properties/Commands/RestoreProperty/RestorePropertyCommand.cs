using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.RestoreProperty;

public record RestorePropertyCommand(Guid Id) : IRequest<Unit>;
