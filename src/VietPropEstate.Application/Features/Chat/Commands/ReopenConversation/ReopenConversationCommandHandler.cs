using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Features.Chat.Commands.ReopenConversation;

public sealed class ReopenConversationCommandHandler : IRequestHandler<ReopenConversationCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly IChatNotificationService _chat;

    public ReopenConversationCommandHandler(IApplicationDbContext db, IChatNotificationService chat)
    {
        _db = db;
        _chat = chat;
    }

    public async Task Handle(ReopenConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Conversation), request.ConversationId);

        if (!conversation.IsParticipant(request.RequestingUserId))
            throw new ForbiddenAccessException();

        conversation.Reopen();
        await _db.SaveChangesAsync(cancellationToken);
        await _chat.SendConversationStatusAsync(request.ConversationId, false, cancellationToken);
    }
}
