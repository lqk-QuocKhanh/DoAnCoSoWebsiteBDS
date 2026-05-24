using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Properties.Commands.ToggleFavorite;
using VietPropEstate.Application.Features.Properties.Queries.GetFavoriteProperties;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>Favorites management — toggle and list saved properties.</summary>
[Authorize]
public class FavoritesController : BaseApiController
{
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _db;

    public FavoritesController(ICurrentUserService currentUser, IApplicationDbContext db)
    {
        _currentUser = currentUser;
        _db = db;
    }

    // ── GET /api/favorites ────────────────────────────────────────────────────

    /// <summary>Returns the authenticated user's favorited properties (paginated).</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavorites(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(
            new GetFavoritePropertiesQuery(_currentUser.UserId!, pageNumber, pageSize),
            cancellationToken));

    // ── POST /api/favorites/{propertyId:guid} ─────────────────────────────────

    /// <summary>Toggle favorite — adds if not saved, removes if already saved.</summary>
    [HttpPost("{propertyId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Toggle(
        Guid propertyId,
        [FromBody] ToggleFavoriteRequest? request,
        CancellationToken cancellationToken)
    {
        var added = await Mediator.Send(new ToggleFavoriteCommand
        {
            UserId = _currentUser.UserId!,
            PropertyId = propertyId,
            Note = request?.Note
        }, cancellationToken);

        return Ok(new { added, message = added ? "Added to favorites." : "Removed from favorites." });
    }

    // ── GET /api/favorites/{propertyId:guid}/status ───────────────────────────

    /// <summary>Check whether the property is in the user's favorites.</summary>
    [HttpGet("{propertyId:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(Guid propertyId, CancellationToken cancellationToken)
    {
        var isFavorited = await _db.Favorites.AnyAsync(
            f => f.UserId == _currentUser.UserId! &&
                 f.PropertyId == propertyId &&
                 !f.IsDeleted,
            cancellationToken);

        return Ok(new { isFavorited });
    }
}

public record ToggleFavoriteRequest(string? Note);
