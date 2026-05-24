using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.DeleteProperty;

public record DeletePropertyCommand(Guid Id, string? RequestedByUserId, bool IsAdmin) : IRequest<Unit>;
