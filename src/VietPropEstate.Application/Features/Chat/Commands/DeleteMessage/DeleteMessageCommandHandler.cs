using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Features.Chat.Commands.DeleteMessage;

public sealed class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand>
{
    private readonly IApplicationDbContext _db;

    public DeleteMessageCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _db.Messages
            .FirstOrDefaultAsync(m => m.Id == request.MessageId && !m.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Message), request.MessageId);

        if (message.SenderId != request.RequestingUserId)
            throw new ForbiddenAccessException();

        message.SoftDelete();
        await _db.SaveChangesAsync(cancellationToken);
    }
}
