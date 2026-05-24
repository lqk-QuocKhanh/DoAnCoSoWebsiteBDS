using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.MarkPropertyAsSold;

public record MarkPropertyAsSoldCommand(Guid Id) : IRequest<Unit>;
