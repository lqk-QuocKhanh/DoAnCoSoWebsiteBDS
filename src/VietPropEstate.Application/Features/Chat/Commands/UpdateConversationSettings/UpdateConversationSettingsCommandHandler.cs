using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Chat;
using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Features.Chat.Commands.UpdateConversationSettings;

public sealed class UpdateConversationSettingsCommandHandler
    : IRequestHandler<UpdateConversationSettingsCommand, ConversationDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IOnlineUserTracker _onlineTracker;
    private readonly IUserAvatarService _avatarService;

    public UpdateConversationSettingsCommandHandler(
        IApplicationDbContext db,
        IOnlineUserTracker onlineTracker,
        IUserAvatarService avatarService)
    {
        _db = db;
        _onlineTracker = onlineTracker;
        _avatarService = avatarService;
    }

    public async Task<ConversationDto> Handle(
        UpdateConversationSettingsCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _db.Conversations
            .Include(c => c.Property)
                .ThenInclude(p => p.Images.Where(i => i.IsPrimary).Take(1))
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Conversation), request.ConversationId);

        if (!conversation.IsParticipant(request.UserId))
            throw new ForbiddenAccessException();

        var setting = await _db.UserConversationSettings
            .FirstOrDefaultAsync(s =>
                s.UserId == request.UserId && s.ConversationId == request.ConversationId,
                cancellationToken);

        if (setting is null && (request.IsPinned == true || request.IsMuted == true))
        {
            setting = UserConversationSetting.Create(request.UserId, request.ConversationId);
            await _db.UserConversationSettings.AddAsync(setting, cancellationToken);
        }

        if (setting is not null)
        {
            if (request.IsPinned.HasValue)
                setting.SetPinned(request.IsPinned.Value);

            if (request.IsMuted.HasValue)
                setting.SetMuted(request.IsMuted.Value);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return await MapConversationDtoAsync(conversation, request.UserId, cancellationToken);
    }

    private async Task<ConversationDto> MapConversationDtoAsync(
        Conversation conversation, string userId, CancellationToken cancellationToken)
    {
        var (otherId, otherName, otherAvatar) = await ConversationParticipantHelper.GetOtherParticipantAsync(
            _db, _avatarService, conversation.BuyerId, conversation.SellerId, userId, cancellationToken);

        var setting = await _db.UserConversationSettings
            .FirstOrDefaultAsync(s => s.UserId == userId && s.ConversationId == conversation.Id,
                cancellationToken);

        var isBlockedByMe = await ChatBlockHelper.HasBlockedAsync(
            _db, userId, otherId, cancellationToken);
        var isBlockedByOther = await ChatBlockHelper.HasBlockedAsync(
            _db, otherId, userId, cancellationToken);

        return new ConversationDto
        {
            Id = conversation.Id,
            PropertyId = conversation.PropertyId,
            PropertyTitle = conversation.Property?.Title,
            PropertySlug = conversation.Property?.Slug,
            PropertyImageUrl = conversation.Property?.Images.FirstOrDefault()?.Url,
            BuyerId = conversation.BuyerId,
            SellerId = conversation.SellerId,
            Subject = conversation.Subject,
            IsClosed = conversation.IsClosed,
            LastMessageAt = conversation.LastMessageAt,
            LastMessagePreview = conversation.LastMessagePreview,
            UnreadCount = conversation.GetUnreadCount(userId),
            IsPinned = setting?.IsPinned ?? false,
            IsMuted = setting?.IsMuted ?? false,
            IsBlockedByMe = isBlockedByMe,
            IsBlockedByOther = isBlockedByOther,
            OtherParticipantId = otherId,
            OtherParticipantName = otherName,
            OtherParticipantAvatarUrl = otherAvatar,
            OtherParticipantOnline = _onlineTracker.IsOnline(otherId),
            CreatedAt = conversation.CreatedAt
        };
    }
}
