using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;

namespace VietPropEstate.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand>
{
    private readonly IApplicationDbContext _db;

    public MarkNotificationAsReadCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId && !n.IsDeleted, cancellationToken)
            ?? throw new NotFoundException(nameof(Notification), request.NotificationId);

        if (notification.UserId != request.UserId)
            throw new ForbiddenAccessException();

        notification.MarkAsRead();
        await _db.SaveChangesAsync(cancellationToken);
    }
}
