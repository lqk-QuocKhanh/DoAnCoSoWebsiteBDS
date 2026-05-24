using MediatR;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.Queries.GetTransactionTypes;

public record GetTransactionTypesQuery : IRequest<IReadOnlyList<TransactionTypeDto>>;
