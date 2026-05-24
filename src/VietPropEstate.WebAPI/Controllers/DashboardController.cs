using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Chat.Queries.GetUnreadCount;
using VietPropEstate.Application.Features.Dashboard.DTOs;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>Dashboard statistics for the authenticated user.</summary>
[Authorize]
public class DashboardController : BaseApiController
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DashboardController(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <summary>Returns overview stats for the user's dashboard.</summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!;

        var agent = await _db.Agents
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId && !a.IsDeleted, cancellationToken);

        var listingsQuery = _db.Properties.Where(p => !p.IsDeleted);
        if (agent is not null)
            listingsQuery = listingsQuery.Where(p => p.AgentId == agent.Id);

        var listings = await listingsQuery
            .AsNoTracking()
            .Select(p => new { p.Status, p.ViewCount })
            .ToListAsync(cancellationToken);

        var favoriteCount = await _db.Favorites
            .CountAsync(f => f.UserId == userId && !f.IsDeleted, cancellationToken);

        var unread = await Mediator.Send(new GetUnreadCountQuery(userId), cancellationToken);

        var stats = new DashboardStatsDto
        {
            TotalListings = listings.Count,
            ActiveListings = listings.Count(p => p.Status == PropertyStatus.Active),
            PendingListings = listings.Count(p => p.Status == PropertyStatus.PendingApproval),
            TotalViews = listings.Sum(p => p.ViewCount),
            TotalFavorites = favoriteCount,
            UnreadMessages = unread.UnreadMessages,
            UnreadNotifications = unread.UnreadNotifications
        };

        return Ok(stats);
    }
}
