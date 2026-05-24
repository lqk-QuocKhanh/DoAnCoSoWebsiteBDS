using MediatR;

namespace VietPropEstate.Application.Features.Payments.Commands.CancelPayment;

public sealed record CancelPaymentCommand(Guid PaymentId, string UserId, string Reason) : IRequest;
