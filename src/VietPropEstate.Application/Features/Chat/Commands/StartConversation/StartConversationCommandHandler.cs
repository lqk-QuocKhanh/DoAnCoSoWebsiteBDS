using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Chat;
using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Features.Chat.Commands.StartConversation;

public sealed class StartConversationCommandHandler
    : IRequestHandler<StartConversationCommand, ConversationDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IChatNotificationService _chat;
    private readonly IUserAvatarService _avatarService;

    public StartConversationCommandHandler(
        IApplicationDbContext db,
        IChatNotificationService chat,
        IUserAvatarService avatarService)
    {
        _db = db;
        _chat = chat;
        _avatarService = avatarService;
    }

    public async Task<ConversationDto> Handle(
        StartConversationCommand request, CancellationToken cancellationToken)
    {
        // Validate property exists
        if (!await _db.Properties.AnyAsync(p => p.Id == request.PropertyId, cancellationToken))
            throw new NotFoundException(nameof(Property), request.PropertyId);

        if (await ChatBlockHelper.IsBlockedBetweenAsync(
                _db, request.BuyerId, request.SellerId, cancellationToken))
            throw new InvalidOperationException("Không thể bắt đầu cuộc trò chuyện với người dùng này.");

        // Idempotent — return existing conversation if one already exists
        var existing = await _db.Conversations
            .FirstOrDefaultAsync(c =>
                c.PropertyId == request.PropertyId &&
                c.BuyerId == request.BuyerId &&
                c.SellerId == request.SellerId &&
                !c.IsDeleted,
                cancellationToken);

        if (existing is not null)
            return await MapConversationDtoAsync(existing, request.BuyerId, cancellationToken);

        // Create new conversation
        var conversation = Conversation.Create(
            request.PropertyId, request.BuyerId, request.SellerId, request.Subject);

        await _db.Conversations.AddAsync(conversation, cancellationToken);

        // Optionally send the first message
        if (!string.IsNullOrWhiteSpace(request.InitialMessage))
        {
            var message = Message.Create(
                conversation.Id, request.BuyerId, request.InitialMessage);
            conversation.UpdateLastMessage(request.InitialMessage, request.BuyerId);
            await _db.Messages.AddAsync(message, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return await MapConversationDtoAsync(conversation, request.BuyerId, cancellationToken);
    }

    private async Task<ConversationDto> MapConversationDtoAsync(
        Conversation c, string requestingUserId, CancellationToken ct)
    {
        var property = await _db.Properties
            .Include(p => p.Images.Where(i => i.IsPrimary).Take(1))
            .FirstOrDefaultAsync(p => p.Id == c.PropertyId, ct);

        var (otherId, otherName, otherAvatar) = await ConversationParticipantHelper.GetOtherParticipantAsync(
            _db, _avatarService, c.BuyerId, c.SellerId, requestingUserId, ct);

        return new ConversationDto
        {
            Id = c.Id,
            PropertyId = c.PropertyId,
            PropertyTitle = property?.Title,
            PropertySlug = property?.Slug,
            PropertyImageUrl = property?.Images.FirstOrDefault()?.Url,
            BuyerId = c.BuyerId,
            SellerId = c.SellerId,
            Subject = c.Subject,
            IsClosed = c.IsClosed,
            LastMessageAt = c.LastMessageAt,
            LastMessagePreview = c.LastMessagePreview,
            UnreadCount = c.GetUnreadCount(requestingUserId),
            OtherParticipantId = otherId,
            OtherParticipantName = otherName,
            OtherParticipantAvatarUrl = otherAvatar,
            CreatedAt = c.CreatedAt
        };
    }
}
