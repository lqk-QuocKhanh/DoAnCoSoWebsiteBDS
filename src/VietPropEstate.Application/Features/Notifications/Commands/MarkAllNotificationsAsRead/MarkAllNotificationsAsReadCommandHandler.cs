using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler
    : IRequestHandler<MarkAllNotificationsAsReadCommand>
{
    private readonly IApplicationDbContext _db;

    public MarkAllNotificationsAsReadCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(
        MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var unread = await _db.Notifications
            .Where(n => n.UserId == request.UserId && !n.IsRead && !n.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var n in unread)
            n.MarkAsRead();

        if (unread.Count > 0)
            await _db.SaveChangesAsync(cancellationToken);
    }
}
