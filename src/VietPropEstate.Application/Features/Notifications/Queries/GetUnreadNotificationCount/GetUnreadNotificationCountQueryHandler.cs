using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Notifications.Queries.GetUnreadNotificationCount;

public sealed class GetUnreadNotificationCountQueryHandler
    : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    private readonly IApplicationDbContext _db;

    public GetUnreadNotificationCountQueryHandler(IApplicationDbContext db) => _db = db;

    public Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
        => _db.Notifications
              .CountAsync(n => n.UserId == request.UserId && !n.IsRead && !n.IsDeleted, cancellationToken);
}
