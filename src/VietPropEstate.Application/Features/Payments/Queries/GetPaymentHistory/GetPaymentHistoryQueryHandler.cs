using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Payments.DTOs;

namespace VietPropEstate.Application.Features.Payments.Queries.GetPaymentHistory;

public sealed class GetPaymentHistoryQueryHandler
    : IRequestHandler<GetPaymentHistoryQuery, PaginatedList<PaymentDto>>
{
    private readonly IApplicationDbContext _db;

    public GetPaymentHistoryQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PaginatedList<PaymentDto>> Handle(
        GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Payments
            .Include(p => p.UserVIPPackage)
                .ThenInclude(u => u != null ? u.VIPPackage : null)
            .Where(p => p.UserId == request.UserId && !p.IsDeleted);

        if (request.Status.HasValue)
            query = query.Where(p => p.Status == request.Status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(p => new PaymentDto
        {
            Id = p.Id,
            UserId = p.UserId,
            VIPPackageId = p.VIPPackageId,
            VIPPackageName = p.UserVIPPackage?.VIPPackage?.Name,
            Amount = p.Amount.Amount,
            Currency = p.Amount.Currency,
            Status = p.Status,
            PaymentMethod = p.PaymentMethod,
            OrderInfo = p.OrderInfo,
            PaymentReference = p.PaymentReference,
            TransactionCode = p.TransactionCode,
            VnpayTransactionId = p.VnpayTransactionId,
            BankCode = p.BankCode,
            PaidAt = p.PaidAt,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt
        }).ToList();

        return PaginatedList<PaymentDto>.Create(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
