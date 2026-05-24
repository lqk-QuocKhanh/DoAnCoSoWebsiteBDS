using MediatR;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Payments.DTOs;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Payments.Queries.GetPaymentHistory;

public sealed record GetPaymentHistoryQuery : IRequest<PaginatedList<PaymentDto>>
{
    public string UserId { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public PaymentStatus? Status { get; init; }
}
