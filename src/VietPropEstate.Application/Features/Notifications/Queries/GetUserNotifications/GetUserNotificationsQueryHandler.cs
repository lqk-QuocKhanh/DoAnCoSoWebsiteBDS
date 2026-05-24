using MediatR;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Notifications.DTOs;

namespace VietPropEstate.Application.Features.Notifications.Queries.GetUserNotifications;

public sealed class GetUserNotificationsQueryHandler
    : IRequestHandler<GetUserNotificationsQuery, PaginatedList<NotificationDto>>
{
    private readonly IApplicationDbContext _db;

    public GetUserNotificationsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PaginatedList<NotificationDto>> Handle(
        GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Notifications
            .Where(n => n.UserId == request.UserId && !n.IsDeleted);

        if (request.UnreadOnly)
            query = query.Where(n => !n.IsRead);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Content = n.Content,
                Type = n.Type,
                IsRead = n.IsRead,
                ReadAt = n.ReadAt,
                ReferenceId = n.ReferenceId,
                ActionUrl = n.ActionUrl,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return PaginatedList<NotificationDto>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }
}
