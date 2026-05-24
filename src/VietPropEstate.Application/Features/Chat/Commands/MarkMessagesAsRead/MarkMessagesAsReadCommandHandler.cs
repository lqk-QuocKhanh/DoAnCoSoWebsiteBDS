using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Chat.Commands.MarkMessagesAsRead;

public sealed class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly IChatNotificationService _chat;

    public MarkMessagesAsReadCommandHandler(
        IApplicationDbContext db, IChatNotificationService chat)
    {
        _db = db;
        _chat = chat;
    }

    public async Task Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Conversation), request.ConversationId);

        if (!conversation.IsParticipant(request.UserId))
            throw new ForbiddenAccessException();

        var now = DateTime.UtcNow;

        // Mark all unread messages from the other participant as Read
        var unreadMessages = await _db.Messages
            .Where(m =>
                m.ConversationId == request.ConversationId &&
                m.SenderId != request.UserId &&
                (m.Status == MessageStatus.Sent || m.Status == MessageStatus.Delivered) &&
                !m.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var msg in unreadMessages)
            msg.MarkAsRead();

        var hadCachedUnread = conversation.GetUnreadCount(request.UserId) > 0;
        if (unreadMessages.Count == 0 && !hadCachedUnread)
            return;

        // Reset cached counter even when messages were already marked read (fixes stale badge counts)
        conversation.ResetUnreadCount(request.UserId);

        await _db.SaveChangesAsync(cancellationToken);

        // Broadcast read receipt to the conversation
        await _chat.SendReadReceiptAsync(request.ConversationId, request.UserId, now, cancellationToken);

        // Push updated unread counts to this user
        var totalUnread = await _db.Conversations
            .Where(c => (c.BuyerId == request.UserId || c.SellerId == request.UserId) && !c.IsDeleted)
            .SumAsync(c => c.BuyerId == request.UserId ? c.BuyerUnreadCount : c.SellerUnreadCount,
                cancellationToken);

        await _chat.SendUnreadCountsAsync(request.UserId, totalUnread, 0, cancellationToken);
    }
}
