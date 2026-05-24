using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;
using VietPropEstate.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using VietPropEstate.Application.Features.Notifications.Queries.GetUnreadNotificationCount;
using VietPropEstate.Application.Features.Notifications.Queries.GetUserNotifications;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>In-app notifications for the authenticated user.</summary>
[Authorize]
public class NotificationsController : BaseApiController
{
    private readonly ICurrentUserService _currentUser;
    public NotificationsController(ICurrentUserService currentUser) => _currentUser = currentUser;

    // ── GET /api/notifications ────────────────────────────────────────────────

    /// <summary>Returns paginated notifications. Pass unreadOnly=true for the badge dropdown.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool unreadOnly = false,
        CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(new GetUserNotificationsQuery
        {
            UserId = _currentUser.UserId!,
            PageNumber = pageNumber,
            PageSize = pageSize,
            UnreadOnly = unreadOnly
        }, cancellationToken));

    // ── GET /api/notifications/count ─────────────────────────────────────────

    /// <summary>Returns the unread notification count (for the badge number).</summary>
    [HttpGet("count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
        => Ok(new { count = await Mediator.Send(
            new GetUnreadNotificationCountQuery(_currentUser.UserId!), cancellationToken) });

    // ── POST /api/notifications/{id:guid}/read ────────────────────────────────

    /// <summary>Mark a single notification as read.</summary>
    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new MarkNotificationAsReadCommand(id, _currentUser.UserId!), cancellationToken);
        return NoContent();
    }

    // ── POST /api/notifications/read-all ─────────────────────────────────────

    /// <summary>Mark all notifications as read (called when user opens the notification panel).</summary>
    [HttpPost("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new MarkAllNotificationsAsReadCommand(_currentUser.UserId!), cancellationToken);
        return NoContent();
    }
}
