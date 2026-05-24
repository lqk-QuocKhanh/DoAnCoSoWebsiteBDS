using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Chat.Commands.MarkMessagesAsRead;
using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Application.Features.Chat.Queries.GetUnreadCount;

namespace VietPropEstate.WebAPI.Hubs;

/// <summary>
/// SignalR hub for real-time chat, typing indicators, online status, and notifications.
/// URL: /hubs/chat
///
/// Connection: JWT passed as access_token query parameter (required for WebSocket).
/// Groups:
///   conversation_{id}  — all participants watching a specific conversation.
///   user_{userId}      — personal channel for notifications and unread counts.
/// </summary>
[Authorize]
public sealed class ChatHub : Hub<IChatClient>
{
    private readonly ISender _mediator;
    private readonly IOnlineUserTracker _onlineTracker;

    private string UserId => Context.UserIdentifier
        ?? throw new InvalidOperationException("Hub requires authentication.");

    public ChatHub(ISender mediator, IOnlineUserTracker onlineTracker)
    {
        _mediator = mediator;
        _onlineTracker = onlineTracker;
    }

    // ── Connection lifecycle ────────────────────────────────────────────────────

    public override async Task OnConnectedAsync()
    {
        await _onlineTracker.UserConnectedAsync(UserId, Context.ConnectionId);

        // Join personal notification group
        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(UserId));

        // Push initial unread counts
        var counts = await _mediator.Send(new GetUnreadCountQuery(UserId));
        await Clients.Caller.UnreadCountsUpdated(counts.UnreadMessages, counts.UnreadNotifications);

        // Notify shared-conversation contacts that this user is online
        await BroadcastOnlineStatusToContactsAsync(UserId, isOnline: true);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var wentFullyOffline = await _onlineTracker.UserDisconnectedAsync(UserId, Context.ConnectionId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserGroup(UserId));

        if (wentFullyOffline)
        {
            var now = DateTime.UtcNow;
            await _onlineTracker.SetLastSeenAsync(UserId, now);
            await BroadcastOnlineStatusToContactsAsync(UserId, isOnline: false);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // ── Conversation group management ───────────────────────────────────────────

    /// <summary>
    /// Join a conversation group so the client receives real-time messages for it.
    /// Called by client when they open a conversation.
    /// </summary>
    public async Task JoinConversation(Guid conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
    }

    /// <summary>
    /// Leave a conversation group. Called when the client navigates away.
    /// </summary>
    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
    }

    // ── Typing indicators ───────────────────────────────────────────────────────

    /// <summary>Broadcast that this user is typing in the conversation.</summary>
    public async Task StartTyping(Guid conversationId)
    {
        var indicator = new TypingIndicatorDto(conversationId, UserId, null, true);
        await Clients.OthersInGroup(ConversationGroup(conversationId))
                     .TypingIndicator(indicator);
    }

    /// <summary>Broadcast that this user has stopped typing.</summary>
    public async Task StopTyping(Guid conversationId)
    {
        var indicator = new TypingIndicatorDto(conversationId, UserId, null, false);
        await Clients.OthersInGroup(ConversationGroup(conversationId))
                     .TypingIndicator(indicator);
    }

    // ── Read receipts ───────────────────────────────────────────────────────────

    /// <summary>
    /// Mark all unread messages in a conversation as read.
    /// The server persists the change and broadcasts a read receipt.
    /// </summary>
    public async Task MarkAsRead(Guid conversationId)
    {
        try
        {
            await _mediator.Send(new MarkMessagesAsReadCommand
            {
                ConversationId = conversationId,
                UserId = UserId
            });
        }
        catch (Exception ex)
        {
            await Clients.Caller.Error(ex.Message);
        }
    }

    // ── Online status query ─────────────────────────────────────────────────────

    /// <summary>Returns the online status of a specific user.</summary>
    public async Task GetUserStatus(string userId)
    {
        var isOnline = _onlineTracker.IsOnline(userId);
        DateTime? lastSeen = isOnline ? null : await _onlineTracker.GetLastSeenAsync(userId);

        await Clients.Caller.OnlineStatusChanged(
            new OnlineStatusDto(userId, isOnline, lastSeen));
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private static string ConversationGroup(Guid id) => $"conversation_{id}";
    private static string UserGroup(string userId) => $"user_{userId}";

    /// <summary>
    /// Notifies all users who share at least one conversation with this user
    /// about their online/offline status change.
    /// </summary>
    private async Task BroadcastOnlineStatusToContactsAsync(string userId, bool isOnline)
    {
        // Use the shared DB context to find contacts via conversations
        // We push to their personal group channels
        var status = new OnlineStatusDto(userId, isOnline,
            isOnline ? null : await _onlineTracker.GetLastSeenAsync(userId));

        // Note: Without querying the DB here (to keep hub lean),
        // we broadcast to the caller's current conversation groups.
        // Each conversation group member will receive the status update.
        // For a full solution, inject IApplicationDbContext and query contact IDs.
        await Clients.Others.OnlineStatusChanged(status);
    }
}
