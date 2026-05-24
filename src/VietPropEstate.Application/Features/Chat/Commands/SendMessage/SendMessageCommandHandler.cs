using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Chat;
using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Chat.Commands.SendMessage;

public sealed class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IChatNotificationService _chat;
    private readonly INotificationService _notifications;
    private readonly IUserAvatarService _avatarService;

    public SendMessageCommandHandler(
        IApplicationDbContext db,
        IChatNotificationService chat,
        INotificationService notifications,
        IUserAvatarService avatarService)
    {
        _db = db;
        _chat = chat;
        _notifications = notifications;
        _avatarService = avatarService;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Conversation), request.ConversationId);

        if (!conversation.IsParticipant(request.SenderId))
            throw new ForbiddenAccessException();

        if (conversation.IsClosed)
            throw new InvalidOperationException("Cannot send messages to a closed conversation.");

        var recipientId = conversation.GetOtherParticipantId(request.SenderId);
        if (await ChatBlockHelper.IsBlockedBetweenAsync(_db, request.SenderId, recipientId, cancellationToken))
            throw new InvalidOperationException("Không thể gửi tin nhắn. Một trong hai người đã chặn cuộc trò chuyện này.");

        // Persist message
        var message = Message.Create(
            request.ConversationId, request.SenderId, request.Content, request.AttachmentUrl);

        conversation.UpdateLastMessage(request.Content, request.SenderId);

        await _db.Messages.AddAsync(message, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        var names = await ChatParticipantLookup.GetDisplayNamesAsync(
            _db, new[] { request.SenderId }, cancellationToken);
        var avatars = await _avatarService.GetAvatarUrlsAsync(new[] { request.SenderId }, cancellationToken);
        names.TryGetValue(request.SenderId, out var senderName);
        avatars.TryGetValue(request.SenderId, out var senderAvatar);

        var dto = new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = senderName,
            SenderAvatarUrl = senderAvatar,
            Content = message.Content,
            AttachmentUrl = message.AttachmentUrl,
            Status = message.Status,
            SentAt = message.CreatedAt,
            IsOwn = true
        };

        // Push real-time message to conversation group
        await _chat.SendMessageToConversationAsync(request.ConversationId, dto, cancellationToken);

        // Deliver ACK to sender
        await _chat.SendDeliveryAckAsync(
            request.SenderId, message.Id, message.CreatedAt, cancellationToken);

        // Create in-app notification for the recipient (skip if muted)
        var isMuted = await _db.UserConversationSettings.AnyAsync(s =>
            s.UserId == recipientId &&
            s.ConversationId == conversation.Id &&
            s.IsMuted,
            cancellationToken);

        if (!isMuted)
        {
            await _notifications.CreateAndPushAsync(
                recipientId,
                "New message",
                $"{request.Content[..Math.Min(100, request.Content.Length)]}",
                NotificationType.MessageReceived,
                referenceId: conversation.Id.ToString(),
                actionUrl: $"/dashboard/tin-nhan?conversation={conversation.Id}",
                cancellationToken: cancellationToken);
        }

        // Push unread count update to recipient
        var unreadMessages = await _db.Conversations
            .Where(c => (c.BuyerId == recipientId || c.SellerId == recipientId) && !c.IsDeleted)
            .SumAsync(c => c.BuyerId == recipientId ? c.BuyerUnreadCount : c.SellerUnreadCount,
                cancellationToken);

        await _chat.SendUnreadCountsAsync(recipientId, unreadMessages, 0, cancellationToken);

        return dto;
    }
}
