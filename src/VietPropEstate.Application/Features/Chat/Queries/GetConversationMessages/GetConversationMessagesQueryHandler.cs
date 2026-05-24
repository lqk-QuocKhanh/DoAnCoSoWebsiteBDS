using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Chat;
using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Chat.Queries.GetConversationMessages;

public sealed class GetConversationMessagesQueryHandler
    : IRequestHandler<GetConversationMessagesQuery, PaginatedList<MessageDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IUserAvatarService _avatarService;

    public GetConversationMessagesQueryHandler(
        IApplicationDbContext db,
        IUserAvatarService avatarService)
    {
        _db = db;
        _avatarService = avatarService;
    }

    public async Task<PaginatedList<MessageDto>> Handle(
        GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Conversation), request.ConversationId);

        if (!conversation.IsParticipant(request.RequestingUserId))
            throw new ForbiddenAccessException();

        var query = _db.Messages
            .Where(m => m.ConversationId == request.ConversationId && !m.IsDeleted);

        if (request.Before.HasValue)
            query = query.Where(m => m.CreatedAt < request.Before.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Mark fetched messages from the other user as Delivered
        var toDeliver = messages
            .Where(m => m.SenderId != request.RequestingUserId && m.Status == MessageStatus.Sent)
            .ToList();

        foreach (var msg in toDeliver)
            msg.MarkAsDelivered();

        if (toDeliver.Count > 0)
            await _db.SaveChangesAsync(cancellationToken);

        var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
        var names = await ChatParticipantLookup.GetDisplayNamesAsync(_db, senderIds, cancellationToken);
        var avatars = await _avatarService.GetAvatarUrlsAsync(senderIds, cancellationToken);

        var dtos = messages
            .OrderBy(m => m.CreatedAt) // Chronological order for display
            .Select(m =>
            {
                names.TryGetValue(m.SenderId, out var senderName);
                avatars.TryGetValue(m.SenderId, out var senderAvatar);
                return new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderName = senderName,
                    SenderAvatarUrl = senderAvatar,
                    Content = m.Content,
                    AttachmentUrl = m.AttachmentUrl,
                    Status = m.Status,
                    ReadAt = m.ReadAt,
                    SentAt = m.CreatedAt,
                    IsOwn = m.SenderId == request.RequestingUserId
                };
            })
            .ToList();

        return PaginatedList<MessageDto>.Create(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
