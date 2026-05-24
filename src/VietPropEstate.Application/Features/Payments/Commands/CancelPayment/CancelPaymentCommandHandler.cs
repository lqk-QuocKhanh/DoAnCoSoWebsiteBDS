using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Payments.Commands.CancelPayment;

public sealed class CancelPaymentCommandHandler : IRequestHandler<CancelPaymentCommand>
{
    private readonly IApplicationDbContext _db;

    public CancelPaymentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _db.Payments
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId && !p.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), request.PaymentId);

        if (payment.UserId != request.UserId)
            throw new ForbiddenAccessException();

        if (payment.Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be cancelled.");

        payment.Cancel(request.Reason);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
