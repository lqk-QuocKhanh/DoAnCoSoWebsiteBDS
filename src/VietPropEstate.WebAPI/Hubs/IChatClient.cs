using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Application.Features.Notifications.DTOs;

namespace VietPropEstate.WebAPI.Hubs;

/// <summary>
/// Strongly-typed contract for events pushed from the server to connected chat clients.
/// </summary>
public interface IChatClient
{
    /// <summary>A new message was sent in a conversation the client is watching.</summary>
    Task ReceiveMessage(MessageDto message);

    /// <summary>Delivery confirmation sent back to the original sender.</summary>
    Task MessageDelivered(Guid messageId, DateTime deliveredAt);

    /// <summary>Read receipt: all messages in this conversation were read by the given user.</summary>
    Task MessagesRead(Guid conversationId, string readByUserId, DateTime readAt);

    /// <summary>Another participant is typing (or has stopped typing).</summary>
    Task TypingIndicator(TypingIndicatorDto indicator);

    /// <summary>Another user's online status has changed.</summary>
    Task OnlineStatusChanged(OnlineStatusDto status);

    /// <summary>A new in-app notification was pushed to this user.</summary>
    Task ReceiveNotification(NotificationDto notification);

    /// <summary>Updated combined unread counts (messages + notifications).</summary>
    Task UnreadCountsUpdated(int unreadMessages, int unreadNotifications);

    /// <summary>A conversation this user participates in was closed or reopened.</summary>
    Task ConversationStatusChanged(Guid conversationId, bool isClosed);

    /// <summary>Server-side error message.</summary>
    Task Error(string message);
}
