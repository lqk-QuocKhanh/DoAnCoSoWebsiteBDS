using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Features.Chat.Commands.UnblockUser;

public sealed class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand>
{
    private readonly IApplicationDbContext _db;

    public UnblockUserCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Conversation), request.ConversationId);

        if (!conversation.IsParticipant(request.UserId))
            throw new ForbiddenAccessException();

        var blockedUserId = conversation.GetOtherParticipantId(request.UserId);

        var block = await _db.UserBlocks
            .FirstOrDefaultAsync(b =>
                b.BlockerId == request.UserId && b.BlockedUserId == blockedUserId,
                cancellationToken);

        if (block is null)
            return;

        _db.UserBlocks.Remove(block);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
