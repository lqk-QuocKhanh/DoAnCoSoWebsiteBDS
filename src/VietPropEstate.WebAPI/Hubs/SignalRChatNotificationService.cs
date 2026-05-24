using Microsoft.AspNetCore.SignalR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Application.Features.Notifications.DTOs;

namespace VietPropEstate.WebAPI.Hubs;

/// <summary>
/// Bridges the Application layer's IChatNotificationService interface with
/// the ASP.NET Core SignalR IHubContext&lt;ChatHub, IChatClient&gt;.
/// Registered in Program.cs so it can reference the ChatHub type.
/// </summary>
public sealed class SignalRChatNotificationService : IChatNotificationService
{
    private readonly IHubContext<ChatHub, IChatClient> _hub;

    public SignalRChatNotificationService(IHubContext<ChatHub, IChatClient> hub)
    {
        _hub = hub;
    }

    private static string ConversationGroup(Guid id) => $"conversation_{id}";
    private static string UserGroup(string userId) => $"user_{userId}";

    public async Task SendMessageToConversationAsync(
        Guid conversationId, MessageDto message, CancellationToken ct = default)
        => await _hub.Clients.Group(ConversationGroup(conversationId))
                     .ReceiveMessage(message);

    public async Task SendDeliveryAckAsync(
        string senderId, Guid messageId, DateTime deliveredAt, CancellationToken ct = default)
        => await _hub.Clients.Group(UserGroup(senderId))
                     .MessageDelivered(messageId, deliveredAt);

    public async Task SendReadReceiptAsync(
        Guid conversationId, string readByUserId, DateTime readAt, CancellationToken ct = default)
        => await _hub.Clients.Group(ConversationGroup(conversationId))
                     .MessagesRead(conversationId, readByUserId, readAt);

    public async Task SendTypingIndicatorAsync(
        Guid conversationId, string typingUserId, string? typingUserName,
        bool isTyping, CancellationToken ct = default)
        => await _hub.Clients.Group(ConversationGroup(conversationId))
                     .TypingIndicator(new TypingIndicatorDto(conversationId, typingUserId, typingUserName, isTyping));

    public async Task SendNotificationToUserAsync(
        string userId, NotificationDto notification, CancellationToken ct = default)
        => await _hub.Clients.Group(UserGroup(userId))
                     .ReceiveNotification(notification);

    public async Task SendUnreadCountsAsync(
        string userId, int unreadMessages, int unreadNotifications, CancellationToken ct = default)
        => await _hub.Clients.Group(UserGroup(userId))
                     .UnreadCountsUpdated(unreadMessages, unreadNotifications);

    public async Task BroadcastOnlineStatusAsync(
        string userId, bool isOnline, IEnumerable<string> toUserIds, CancellationToken ct = default)
    {
        var status = new OnlineStatusDto(userId, isOnline, isOnline ? null : DateTime.UtcNow);
        var tasks = toUserIds.Select(id =>
            _hub.Clients.Group(UserGroup(id)).OnlineStatusChanged(status));
        await Task.WhenAll(tasks);
    }

    public async Task SendConversationStatusAsync(
        Guid conversationId, bool isClosed, CancellationToken ct = default)
        => await _hub.Clients.Group(ConversationGroup(conversationId))
                     .ConversationStatusChanged(conversationId, isClosed);
}
