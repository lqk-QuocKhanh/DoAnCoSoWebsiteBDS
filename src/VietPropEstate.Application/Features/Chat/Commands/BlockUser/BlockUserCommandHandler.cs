using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Features.Chat.Commands.BlockUser;

public sealed class BlockUserCommandHandler : IRequestHandler<BlockUserCommand>
{
    private readonly IApplicationDbContext _db;

    public BlockUserCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Conversation), request.ConversationId);

        if (!conversation.IsParticipant(request.UserId))
            throw new ForbiddenAccessException();

        var blockedUserId = conversation.GetOtherParticipantId(request.UserId);

        var existing = await _db.UserBlocks
            .FirstOrDefaultAsync(b =>
                b.BlockerId == request.UserId && b.BlockedUserId == blockedUserId,
                cancellationToken);

        if (existing is not null)
            return;

        await _db.UserBlocks.AddAsync(
            UserBlock.Create(request.UserId, blockedUserId), cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
