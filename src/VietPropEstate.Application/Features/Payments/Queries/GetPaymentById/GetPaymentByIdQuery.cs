using MediatR;
using VietPropEstate.Application.Features.Payments.DTOs;

namespace VietPropEstate.Application.Features.Payments.Queries.GetPaymentById;

public sealed record GetPaymentByIdQuery(Guid PaymentId, string RequestingUserId)
    : IRequest<PaymentDto>;
