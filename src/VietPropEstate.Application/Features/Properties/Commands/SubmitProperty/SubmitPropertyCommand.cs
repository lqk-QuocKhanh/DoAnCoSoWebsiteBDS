using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.SubmitProperty;

public record SubmitPropertyCommand(Guid Id) : IRequest<Unit>;
