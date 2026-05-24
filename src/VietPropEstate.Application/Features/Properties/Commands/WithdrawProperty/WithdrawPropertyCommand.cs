using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.WithdrawProperty;

public record WithdrawPropertyCommand(Guid Id) : IRequest<Unit>;
