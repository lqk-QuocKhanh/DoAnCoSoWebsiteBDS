using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Chat;
using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Features.Chat.Queries.GetConversationById;

public sealed class GetConversationByIdQueryHandler
    : IRequestHandler<GetConversationByIdQuery, ConversationDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IOnlineUserTracker _onlineTracker;
    private readonly IUserAvatarService _avatarService;

    public GetConversationByIdQueryHandler(
        IApplicationDbContext db,
        IOnlineUserTracker onlineTracker,
        IUserAvatarService avatarService)
    {
        _db = db;
        _onlineTracker = onlineTracker;
        _avatarService = avatarService;
    }

    public async Task<ConversationDto> Handle(
        GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        var c = await _db.Conversations
            .Include(x => x.Property)
                .ThenInclude(p => p.Images.Where(i => i.IsPrimary).Take(1))
            .FirstOrDefaultAsync(x => x.Id == request.ConversationId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Conversation), request.ConversationId);

        if (!c.IsParticipant(request.RequestingUserId))
            throw new ForbiddenAccessException();

        var (otherId, otherName, otherAvatar) = await ConversationParticipantHelper.GetOtherParticipantAsync(
            _db, _avatarService, c.BuyerId, c.SellerId, request.RequestingUserId, cancellationToken);

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
            UnreadCount = c.GetUnreadCount(request.RequestingUserId),
            OtherParticipantId = otherId,
            OtherParticipantName = otherName,
            OtherParticipantAvatarUrl = otherAvatar,
            OtherParticipantOnline = _onlineTracker.IsOnline(otherId),
            CreatedAt = c.CreatedAt
        };
    }
}
