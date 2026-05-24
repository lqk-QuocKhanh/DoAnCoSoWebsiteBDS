using MediatR;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Notifications.DTOs;

namespace VietPropEstate.Application.Features.Notifications.Queries.GetUserNotifications;

public sealed record GetUserNotificationsQuery : IRequest<PaginatedList<NotificationDto>>
{
    public string UserId { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool UnreadOnly { get; init; } = false;
}
