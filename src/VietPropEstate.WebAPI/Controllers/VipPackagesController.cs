using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Payments.Queries.GetUserVipPackages;
using VietPropEstate.Application.Features.Payments.Queries.GetVipPackages;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>VIP package catalogue and user subscription management.</summary>
[Route("api/vip-packages")]
public class VipPackagesController : BaseApiController
{
    private readonly ICurrentUserService _currentUser;
    public VipPackagesController(ICurrentUserService currentUser) => _currentUser = currentUser;

    // ── GET /api/vip-packages ─────────────────────────────────────────────────

    /// <summary>Returns all active VIP packages available for purchase.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPackages(CancellationToken cancellationToken)
        => Ok(await Mediator.Send(new GetVipPackagesQuery(), cancellationToken));

    // ── GET /api/vip-packages/my-subscriptions ────────────────────────────────

    /// <summary>Returns the current user's VIP subscriptions.</summary>
    [HttpGet("my-subscriptions")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMySubscriptions(
        [FromQuery] bool activeOnly = true,
        CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(
            new GetUserVipPackagesQuery(_currentUser.UserId!, activeOnly), cancellationToken));
}
