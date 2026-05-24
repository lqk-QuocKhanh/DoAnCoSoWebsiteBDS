using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Payments.DTOs;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Features.Payments.Queries.GetPaymentById;

public sealed class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
{
    private readonly IApplicationDbContext _db;

    public GetPaymentByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PaymentDto> Handle(
        GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _db.Payments
            .Include(p => p.UserVIPPackage)
                .ThenInclude(u => u != null ? u.VIPPackage : null)
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId && !p.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), request.PaymentId);

        // Users can only view their own payments; admins can view any
        if (payment.UserId != request.RequestingUserId)
            throw new ForbiddenAccessException();

        return new PaymentDto
        {
            Id = payment.Id,
            UserId = payment.UserId,
            VIPPackageId = payment.VIPPackageId,
            VIPPackageName = payment.UserVIPPackage?.VIPPackage?.Name,
            Amount = payment.Amount.Amount,
            Currency = payment.Amount.Currency,
            Status = payment.Status,
            PaymentMethod = payment.PaymentMethod,
            OrderInfo = payment.OrderInfo,
            PaymentReference = payment.PaymentReference,
            TransactionCode = payment.TransactionCode,
            VnpayTransactionId = payment.VnpayTransactionId,
            BankCode = payment.BankCode,
            PaidAt = payment.PaidAt,
            Notes = payment.Notes,
            CreatedAt = payment.CreatedAt
        };
    }
}
