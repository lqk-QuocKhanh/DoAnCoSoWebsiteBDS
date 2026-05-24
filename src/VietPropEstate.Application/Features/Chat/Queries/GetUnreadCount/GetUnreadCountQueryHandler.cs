using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Chat.Queries.GetUnreadCount;

public sealed class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, UnreadCountResult>
{
    private readonly IApplicationDbContext _db;

    public GetUnreadCountQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<UnreadCountResult> Handle(
        GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        var unreadMessages = await _db.Conversations
            .Where(c => (c.BuyerId == request.UserId || c.SellerId == request.UserId) && !c.IsDeleted)
            .SumAsync(c => c.BuyerId == request.UserId ? c.BuyerUnreadCount : c.SellerUnreadCount,
                cancellationToken);

        var unreadNotifications = await _db.Notifications
            .CountAsync(n => n.UserId == request.UserId && !n.IsRead && !n.IsDeleted, cancellationToken);

        return new UnreadCountResult(unreadMessages, unreadNotifications);
    }
}
