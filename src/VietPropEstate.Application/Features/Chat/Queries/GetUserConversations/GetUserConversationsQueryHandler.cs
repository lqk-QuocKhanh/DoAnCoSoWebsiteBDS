using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Chat;
using VietPropEstate.Application.Features.Chat.DTOs;

namespace VietPropEstate.Application.Features.Chat.Queries.GetUserConversations;

public sealed class GetUserConversationsQueryHandler
    : IRequestHandler<GetUserConversationsQuery, PaginatedList<ConversationDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IOnlineUserTracker _onlineTracker;
    private readonly IUserAvatarService _avatarService;

    public GetUserConversationsQueryHandler(
        IApplicationDbContext db,
        IOnlineUserTracker onlineTracker,
        IUserAvatarService avatarService)
    {
        _db = db;
        _onlineTracker = onlineTracker;
        _avatarService = avatarService;
    }

    public async Task<PaginatedList<ConversationDto>> Handle(
        GetUserConversationsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Conversations
            .Include(c => c.Property)
                .ThenInclude(p => p.Images.Where(i => i.IsPrimary).Take(1))
            .Where(c => (c.BuyerId == request.UserId || c.SellerId == request.UserId) && !c.IsDeleted);

        if (!request.IncludeClosed)
            query = query.Where(c => !c.IsClosed);

        var settings = await _db.UserConversationSettings
            .Where(s => s.UserId == request.UserId)
            .ToListAsync(cancellationToken);
        var settingsByConversation = settings.ToDictionary(s => s.ConversationId);

        var blocks = await _db.UserBlocks
            .Where(b => b.BlockerId == request.UserId || b.BlockedUserId == request.UserId)
            .ToListAsync(cancellationToken);

        var allConversations = await query.ToListAsync(cancellationToken);

        var sorted = allConversations
            .OrderByDescending(c => settingsByConversation.GetValueOrDefault(c.Id)?.IsPinned ?? false)
            .ThenByDescending(c => settingsByConversation.GetValueOrDefault(c.Id)?.PinnedAt)
            .ThenByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .ToList();

        var totalCount = sorted.Count;
        var conversations = sorted
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var participantIds = conversations
            .SelectMany(c => new[] { c.BuyerId, c.SellerId })
            .Distinct()
            .ToList();
        var names = await ChatParticipantLookup.GetDisplayNamesAsync(_db, participantIds, cancellationToken);
        var avatars = await _avatarService.GetAvatarUrlsAsync(participantIds, cancellationToken);

        var dtos = conversations.Select(c =>
        {
            var otherId = c.GetOtherParticipantId(request.UserId);
            names.TryGetValue(otherId, out var otherName);
            avatars.TryGetValue(otherId, out var otherAvatar);
            var setting = settingsByConversation.GetValueOrDefault(c.Id);

            return new ConversationDto
            {
                Id = c.Id,
                PropertyId = c.PropertyId,
                PropertyTitle = c.Property?.Title,
                PropertySlug = c.Property?.Slug,
                PropertyImageUrl = c.Property?.Images.FirstOrDefault()?.Url,
                BuyerId = c.BuyerId,
                SellerId = c.SellerId,
                Subject = c.Subject,
                IsClosed = c.IsClosed,
                LastMessageAt = c.LastMessageAt,
                LastMessagePreview = c.LastMessagePreview,
                UnreadCount = c.GetUnreadCount(request.UserId),
                IsPinned = setting?.IsPinned ?? false,
                IsMuted = setting?.IsMuted ?? false,
                IsBlockedByMe = blocks.Any(b => b.BlockerId == request.UserId && b.BlockedUserId == otherId),
                IsBlockedByOther = blocks.Any(b => b.BlockerId == otherId && b.BlockedUserId == request.UserId),
                OtherParticipantId = otherId,
                OtherParticipantName = otherName,
                OtherParticipantAvatarUrl = otherAvatar,
                OtherParticipantOnline = _onlineTracker.IsOnline(otherId),
                CreatedAt = c.CreatedAt
            };
        }).ToList();

        return PaginatedList<ConversationDto>.Create(
            dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
