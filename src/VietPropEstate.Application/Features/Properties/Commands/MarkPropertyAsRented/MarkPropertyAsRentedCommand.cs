using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.MarkPropertyAsRented;

public record MarkPropertyAsRentedCommand(Guid Id) : IRequest<Unit>;
