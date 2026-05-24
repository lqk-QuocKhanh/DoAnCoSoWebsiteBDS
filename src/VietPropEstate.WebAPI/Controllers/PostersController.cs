using Microsoft.AspNetCore.Mvc;
using VietPropEstate.Application.Features.Properties.Queries.GetPosterProfile;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>Public poster/seller profile pages.</summary>
[Route("api/posters")]
public class PostersController : BaseApiController
{
    /// <summary>Returns public profile and published listings for a user.</summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(
        string userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var profile = await Mediator.Send(
            new GetPosterProfileQuery(userId, pageNumber, pageSize), cancellationToken);

        return profile is null ? NotFound() : Ok(profile);
    }
}
