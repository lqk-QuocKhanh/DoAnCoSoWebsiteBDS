using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Chat.Commands.BlockUser;
using VietPropEstate.Application.Features.Chat.Commands.CloseConversation;
using VietPropEstate.Application.Features.Chat.Commands.DeleteMessage;
using VietPropEstate.Application.Features.Chat.Commands.MarkMessagesAsRead;
using VietPropEstate.Application.Features.Chat.Commands.ReopenConversation;
using VietPropEstate.Application.Features.Chat.Commands.SendMessage;
using VietPropEstate.Application.Features.Chat.Commands.StartConversation;
using VietPropEstate.Application.Features.Chat.Commands.StartSupportConversation;
using VietPropEstate.Application.Features.Chat.Commands.UnblockUser;
using VietPropEstate.Application.Features.Chat.Commands.UpdateConversationSettings;
using VietPropEstate.Application.Features.Chat.Queries.GetConversationById;
using VietPropEstate.Application.Features.Chat.Queries.GetConversationMessages;
using VietPropEstate.Application.Features.Chat.Queries.GetUnreadCount;
using VietPropEstate.Application.Features.Chat.Queries.GetUserConversations;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>Buyer–Seller conversation REST API. SignalR hub: /hubs/chat</summary>
[Authorize]
public class ConversationsController : BaseApiController
{
    private readonly ICurrentUserService _currentUser;
    private readonly IConfiguration _configuration;

    public ConversationsController(ICurrentUserService currentUser, IConfiguration configuration)
    {
        _currentUser = currentUser;
        _configuration = configuration;
    }

    // ── GET /api/conversations ────────────────────────────────────────────────

    /// <summary>Returns all conversations for the authenticated user, sorted by latest activity.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyConversations(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeClosed = false,
        CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(new GetUserConversationsQuery
        {
            UserId = _currentUser.UserId!,
            PageNumber = pageNumber,
            PageSize = pageSize,
            IncludeClosed = includeClosed
        }, cancellationToken));

    // ── GET /api/conversations/unread ─────────────────────────────────────────

    /// <summary>Returns unread message + notification counts for the badge indicator.</summary>
    [HttpGet("unread")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCounts(CancellationToken cancellationToken)
        => Ok(await Mediator.Send(new GetUnreadCountQuery(_currentUser.UserId!), cancellationToken));

    // ── GET /api/conversations/{id:guid} ──────────────────────────────────────

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await Mediator.Send(
            new GetConversationByIdQuery(id, _currentUser.UserId!), cancellationToken));

    // ── POST /api/conversations ───────────────────────────────────────────────

    /// <summary>Start (or resume) a conversation between buyer and seller about a property.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Start(
        [FromBody] StartConversationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new StartConversationCommand
        {
            PropertyId = request.PropertyId,
            BuyerId = _currentUser.UserId!,
            SellerId = request.SellerId,
            Subject = request.Subject,
            InitialMessage = request.InitialMessage
        }, cancellationToken);

        return Ok(result);
    }

    /// <summary>Start (or resume) a support conversation with the platform admin.</summary>
    [HttpPost("support")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartSupport(CancellationToken cancellationToken)
        => Ok(await Mediator.Send(new StartSupportConversationCommand
        {
            UserId = _currentUser.UserId!,
            AdminEmail = _configuration["AdminSeed:Email"]
        }, cancellationToken));

    // ── GET /api/conversations/{id:guid}/messages ─────────────────────────────

    /// <summary>Returns paginated message history. Marks delivered messages as delivered.</summary>
    [HttpGet("{id:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] DateTime? before = null,
        CancellationToken cancellationToken = default)
        => Ok(await Mediator.Send(new GetConversationMessagesQuery
        {
            ConversationId = id,
            RequestingUserId = _currentUser.UserId!,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Before = before
        }, cancellationToken));

    // ── POST /api/conversations/{id:guid}/messages ────────────────────────────

    /// <summary>Send a new message. Pushes via SignalR to conversation participants.</summary>
    [HttpPost("{id:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMessage(
        Guid id,
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await Mediator.Send(new SendMessageCommand
        {
            ConversationId = id,
            SenderId = _currentUser.UserId!,
            Content = request.Content,
            AttachmentUrl = request.AttachmentUrl
        }, cancellationToken);

        return CreatedAtAction(nameof(GetMessages), new { id }, dto);
    }

    // ── POST /api/conversations/{id:guid}/read ────────────────────────────────

    /// <summary>Mark all unread messages in this conversation as read.</summary>
    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new MarkMessagesAsReadCommand
        {
            ConversationId = id,
            UserId = _currentUser.UserId!
        }, cancellationToken);
        return NoContent();
    }

    // ── DELETE /api/conversations/{id:guid}/messages/{messageId:guid} ─────────

    /// <summary>Soft-delete a message (replaces content with "[Message deleted]").</summary>
    [HttpDelete("{id:guid}/messages/{messageId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMessage(
        Guid id, Guid messageId, CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new DeleteMessageCommand(messageId, _currentUser.UserId!), cancellationToken);
        return NoContent();
    }

    // ── PUT /api/conversations/{id:guid}/settings ─────────────────────────────

    /// <summary>Update per-user conversation settings (pin, mute).</summary>
    [HttpPut("{id:guid}/settings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSettings(
        Guid id,
        [FromBody] UpdateConversationSettingsRequest request,
        CancellationToken cancellationToken)
        => Ok(await Mediator.Send(new UpdateConversationSettingsCommand
        {
            ConversationId = id,
            UserId = _currentUser.UserId!,
            IsPinned = request.IsPinned,
            IsMuted = request.IsMuted
        }, cancellationToken));

    // ── POST /api/conversations/{id:guid}/block ───────────────────────────────

    /// <summary>Block the other participant in this conversation.</summary>
    [HttpPost("{id:guid}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Block(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new BlockUserCommand
        {
            ConversationId = id,
            UserId = _currentUser.UserId!
        }, cancellationToken);
        return NoContent();
    }

    // ── DELETE /api/conversations/{id:guid}/block ─────────────────────────────

    /// <summary>Unblock the other participant in this conversation.</summary>
    [HttpDelete("{id:guid}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Unblock(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new UnblockUserCommand
        {
            ConversationId = id,
            UserId = _currentUser.UserId!
        }, cancellationToken);
        return NoContent();
    }

    // ── POST /api/conversations/{id:guid}/close ───────────────────────────────

    [HttpPost("{id:guid}/close")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Close(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new CloseConversationCommand(id, _currentUser.UserId!), cancellationToken);
        return NoContent();
    }

    // ── POST /api/conversations/{id:guid}/reopen ──────────────────────────────

    [HttpPost("{id:guid}/reopen")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Reopen(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new ReopenConversationCommand(id, _currentUser.UserId!), cancellationToken);
        return NoContent();
    }
}

// ── Request models ─────────────────────────────────────────────────────────────
public record StartConversationRequest(
    Guid PropertyId, string SellerId, string? Subject, string? InitialMessage);

public record SendMessageRequest(string Content, string? AttachmentUrl);

public record UpdateConversationSettingsRequest(bool? IsPinned, bool? IsMuted);
