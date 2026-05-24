using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Application.Features.Notifications.DTOs;

namespace VietPropEstate.Application.Common.Interfaces;

/// <summary>
/// Pushes real-time events to connected clients via SignalR.
/// Implemented in the WebAPI layer using IHubContext&lt;ChatHub&gt;.
/// </summary>
public interface IChatNotificationService
{
    /// <summary>Broadcasts a new message to all clients in the conversation group.</summary>
    Task SendMessageToConversationAsync(
        Guid conversationId, MessageDto message, CancellationToken ct = default);

    /// <summary>Notifies the sender that their message was delivered.</summary>
    Task SendDeliveryAckAsync(
        string senderId, Guid messageId, DateTime deliveredAt, CancellationToken ct = default);

    /// <summary>Broadcasts a read receipt to all participants in the conversation.</summary>
    Task SendReadReceiptAsync(
        Guid conversationId, string readByUserId, DateTime readAt, CancellationToken ct = default);

    /// <summary>Broadcasts a typing indicator within the conversation (excluding the typer).</summary>
    Task SendTypingIndicatorAsync(
        Guid conversationId, string typingUserId, string? typingUserName,
        bool isTyping, CancellationToken ct = default);

    /// <summary>Pushes an in-app notification to a specific user's personal channel.</summary>
    Task SendNotificationToUserAsync(
        string userId, NotificationDto notification, CancellationToken ct = default);

    /// <summary>Pushes an updated unread counts payload to a specific user.</summary>
    Task SendUnreadCountsAsync(
        string userId, int unreadMessages, int unreadNotifications, CancellationToken ct = default);

    /// <summary>Broadcasts online/offline status change to users who share conversations with this user.</summary>
    Task BroadcastOnlineStatusAsync(
        string userId, bool isOnline, IEnumerable<string> toUserIds, CancellationToken ct = default);

    /// <summary>Notifies all participants that a conversation was closed or reopened.</summary>
    Task SendConversationStatusAsync(
        Guid conversationId, bool isClosed, CancellationToken ct = default);
}
