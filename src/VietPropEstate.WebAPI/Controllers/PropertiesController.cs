using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Properties.Commands.AddPropertyImage;
using VietPropEstate.Application.Features.Properties.Commands.CreateProperty;
using VietPropEstate.Application.Features.Properties.Commands.DeleteProperty;
using VietPropEstate.Application.Features.Properties.Commands.MarkPropertyAsRented;
using VietPropEstate.Application.Features.Properties.Commands.MarkPropertyAsSold;
using VietPropEstate.Application.Features.Properties.Commands.PublishProperty;
using VietPropEstate.Application.Features.Properties.Commands.RemovePropertyImage;
using VietPropEstate.Application.Features.Properties.Commands.RejectProperty;
using VietPropEstate.Application.Features.Properties.Commands.RestoreProperty;
using VietPropEstate.Application.Features.Properties.Commands.SetPropertyFeatured;
using VietPropEstate.Application.Features.Properties.Commands.SubmitProperty;
using VietPropEstate.Application.Features.Properties.Commands.TrackPropertyView;
using VietPropEstate.Application.Features.Properties.Commands.UpdateProperty;
using VietPropEstate.Application.Features.Properties.Commands.UpdatePropertyAddress;
using VietPropEstate.Application.Features.Properties.Commands.WithdrawProperty;
using VietPropEstate.Application.Features.Properties.Queries.GetFeaturedProperties;
using VietPropEstate.Application.Features.Properties.Queries.GetMyProperties;
using VietPropEstate.Application.Features.Properties.Queries.GetPropertiesByAgent;
using VietPropEstate.Application.Features.Properties.Queries.GetPropertiesList;
using VietPropEstate.Application.Features.Properties.Queries.GetPropertyById;
using VietPropEstate.Application.Features.Properties.Queries.GetPropertyBySlug;
using VietPropEstate.Application.Features.Properties.Queries.GetRecentlyViewed;
using VietPropEstate.Application.Features.Properties.Queries.GetRelatedProperties;
using VietPropEstate.Domain.Enums;
using VietPropEstate.WebAPI.Authorization;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>Property listing management — CRUD, search, filtering, and discovery.</summary>
public class PropertiesController : BaseApiController
{
    private readonly ICurrentUserService _currentUser;
    private readonly IWebHostEnvironment _environment;

    public PropertiesController(ICurrentUserService currentUser, IWebHostEnvironment environment)
    {
        _currentUser = currentUser;
        _environment = environment;
    }

    // ── GET /api/properties ────────────────────────────────────────────────────

    /// <summary>Search and filter properties with full pagination and sorting.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? propertyTypeId = null,
        [FromQuery] Guid? transactionTypeId = null,
        [FromQuery] ListingType? listingType = null,
        [FromQuery] PropertyStatus? status = null,
        [FromQuery] int? provinceCode = null,
        [FromQuery] int? wardCode = null,
        [FromQuery] string? provinceName = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] decimal? minArea = null,
        [FromQuery] decimal? maxArea = null,
        [FromQuery] int? minBedrooms = null,
        [FromQuery] int? maxBedrooms = null,
        [FromQuery] int? bathrooms = null,
        [FromQuery] PropertyDirection? direction = null,
        [FromQuery] bool? isFeatured = null,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] string sortOrder = "Desc",
        CancellationToken cancellationToken = default)
    {
        var query = new GetPropertiesListQuery
        {
            PageNumber = pageNumber, PageSize = pageSize,
            SearchTerm = searchTerm,
            PropertyTypeId = propertyTypeId, TransactionTypeId = transactionTypeId,
            ListingType = listingType, Status = status,
            ProvinceCode = provinceCode, WardCode = wardCode, ProvinceName = provinceName,
            MinPrice = minPrice, MaxPrice = maxPrice,
            MinArea = minArea, MaxArea = maxArea,
            MinBedrooms = minBedrooms, MaxBedrooms = maxBedrooms,
            Bathrooms = bathrooms, Direction = direction,
            IsFeatured = isFeatured, SortBy = sortBy, SortOrder = sortOrder
        };
        return Ok(await Mediator.Send(query, cancellationToken));
    }

    // ── GET /api/properties/featured ──────────────────────────────────────────

    /// <summary>Returns the top N featured active listings.</summary>
    [HttpGet("featured")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeatured(
        [FromQuery] int count = 8, CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(new GetFeaturedPropertiesQuery(count), cancellationToken));

    // ── GET /api/properties/my-listings ─────────────────────────────────────

    /// <summary>Returns paginated listings for the authenticated user.</summary>
    [HttpGet("my-listings")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyListings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] PropertyStatus? status = null,
        CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(
            new GetMyPropertiesQuery(pageNumber, pageSize, status), cancellationToken));

    // ── GET /api/properties/{id:guid} ─────────────────────────────────────────

    /// <summary>Get full property detail by ID. Tracks an anonymous view.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetPropertyByIdQuery(id), cancellationToken);
        _ = TrackView(id, cancellationToken); // fire-and-forget
        return Ok(result);
    }

    // ── GET /api/properties/slug/{slug} ──────────────────────────────────────

    /// <summary>Get full property detail by SEO slug.</summary>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetPropertyBySlugQuery(slug), cancellationToken);
        _ = TrackView(result.Id, cancellationToken);
        return Ok(result);
    }

    // ── GET /api/properties/{id:guid}/related ─────────────────────────────────

    /// <summary>Returns related listings (same type + province).</summary>
    [HttpGet("{id:guid}/related")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRelated(
        Guid id, [FromQuery] int count = 6, CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(new GetRelatedPropertiesQuery(id, count), cancellationToken));

    // ── GET /api/properties/agent/{agentId:guid} ──────────────────────────────

    /// <summary>Get paginated properties listed by a specific agent.</summary>
    [HttpGet("agent/{agentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByAgent(
        Guid agentId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(
            new GetPropertiesByAgentQuery(agentId, pageNumber, pageSize), cancellationToken));

    // ── GET /api/properties/recently-viewed ───────────────────────────────────

    /// <summary>Returns the authenticated user's recently viewed properties.</summary>
    [HttpGet("recently-viewed")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentlyViewed(
        [FromQuery] int count = 10, CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(
            new GetRecentlyViewedQuery(_currentUser.UserId!, count), cancellationToken));

    // ── POST /api/properties ──────────────────────────────────────────────────

    /// <summary>Create a new property listing (Customer/Broker; requires admin approval to publish).</summary>
    [HttpPost]
    [Authorize(Policy = AuthPolicies.ListingPublisher)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePropertyCommand command, CancellationToken cancellationToken)
    {
        var id = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    // ── PUT /api/properties/{id:guid} ─────────────────────────────────────────

    /// <summary>Update property details.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdatePropertyCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("Route ID does not match command ID.");
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    // ── DELETE /api/properties/{id:guid} ──────────────────────────────────────

    /// <summary>Soft-delete a property (owner broker or admin).</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeletePropertyCommand(
            id, _currentUser.UserId, _currentUser.IsInRole("Admin")), cancellationToken);
        return NoContent();
    }

    // ── Status transitions ─────────────────────────────────────────────────────

    [HttpPost("{id:guid}/submit")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new SubmitPropertyCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = AuthPolicies.ModeratorOrAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new PublishPropertyCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/withdraw")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Withdraw(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new WithdrawPropertyCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new RestorePropertyCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/mark-sold")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsSold(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new MarkPropertyAsSoldCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/mark-rented")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsRented(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new MarkPropertyAsRentedCommand(id), cancellationToken);
        return NoContent();
    }

    // ── Featured ───────────────────────────────────────────────────────────────

    [HttpPatch("{id:guid}/featured")]
    [Authorize(Policy = AuthPolicies.StaffOrAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetFeatured(
        Guid id, [FromBody] SetFeaturedRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new SetPropertyFeaturedCommand(id, request.IsFeatured), cancellationToken);
        return NoContent();
    }

    // ── Images ─────────────────────────────────────────────────────────────────

    [HttpPost("{id:guid}/images")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddImage(
        Guid id, [FromBody] AddPropertyImageCommand command, CancellationToken cancellationToken)
    {
        var cmd = command with { PropertyId = id };
        var imageId = await Mediator.Send(cmd, cancellationToken);
        return Created($"/api/properties/{id}/images/{imageId}", new { imageId });
    }

    /// <summary>Upload property images (multipart form).</summary>
    [HttpPost("{id:guid}/images/upload")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [RequestSizeLimit(52_428_800)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadImages(
        Guid id,
        [FromForm] List<IFormFile> files,
        CancellationToken cancellationToken)
    {
        if (files.Count == 0)
            return BadRequest(new { message = "No files uploaded." });

        var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var uploadDir = Path.Combine(webRoot, "uploads", "properties", id.ToString());
        Directory.CreateDirectory(uploadDir);

        var uploaded = new List<object>();

        foreach (var file in files.Take(10))
        {
            if (file.Length == 0)
                continue;

            var extension = ResolveImageExtension(file);
            if (extension is null)
                continue;

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadDir, fileName);

            await using (var stream = System.IO.File.Create(filePath))
                await file.CopyToAsync(stream, cancellationToken);

            var url = $"/uploads/properties/{id}/{fileName}";
            var imageId = await Mediator.Send(new AddPropertyImageCommand
            {
                PropertyId = id,
                Url = url
            }, cancellationToken);

            uploaded.Add(new { imageId, url });
        }

        if (uploaded.Count == 0)
            return BadRequest(new { message = "No valid image files were uploaded." });

        return Ok(uploaded);
    }

    private static string? ResolveImageExtension(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension is ".jpg" or ".jpeg" or ".png" or ".webp")
            return extension == ".jpeg" ? ".jpg" : extension;

        return file.ContentType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => null
        };
    }

    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveImage(
        Guid id, Guid imageId, CancellationToken cancellationToken)
    {
        await Mediator.Send(new RemovePropertyImageCommand(id, imageId), cancellationToken);
        return NoContent();
    }

    // ── Address ────────────────────────────────────────────────────────────────

    [HttpPatch("{id:guid}/address")]
    [Authorize(Policy = AuthPolicies.AuthenticatedUser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAddress(
        Guid id, [FromBody] UpdatePropertyAddressCommand command, CancellationToken cancellationToken)
    {
        var cmd = command with { PropertyId = id };
        await Mediator.Send(cmd, cancellationToken);
        return NoContent();
    }

    // ── Private ────────────────────────────────────────────────────────────────

    private Task TrackView(Guid propertyId, CancellationToken cancellationToken)
        => Mediator.Send(new TrackPropertyViewCommand
        {
            PropertyId = propertyId,
            UserId = _currentUser.UserId,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers.UserAgent.FirstOrDefault(),
            SessionId = HttpContext.Session.Id
        }, cancellationToken);
}

public record SetFeaturedRequest(bool IsFeatured);
