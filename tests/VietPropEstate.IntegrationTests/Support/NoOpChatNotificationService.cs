using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Chat.DTOs;
using VietPropEstate.Application.Features.Notifications.DTOs;

namespace VietPropEstate.IntegrationTests.Support;

internal sealed class NoOpChatNotificationService : IChatNotificationService
{
    public Task SendMessageToConversationAsync(Guid conversationId, MessageDto message, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task SendDeliveryAckAsync(string senderId, Guid messageId, DateTime deliveredAt, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task SendReadReceiptAsync(Guid conversationId, string readByUserId, DateTime readAt, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task SendTypingIndicatorAsync(Guid conversationId, string typingUserId, string? typingUserName, bool isTyping, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task SendNotificationToUserAsync(string userId, NotificationDto notification, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task SendUnreadCountsAsync(string userId, int unreadMessages, int unreadNotifications, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task BroadcastOnlineStatusAsync(string userId, bool isOnline, IEnumerable<string> toUserIds, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task SendConversationStatusAsync(Guid conversationId, bool isClosed, CancellationToken ct = default)
        => Task.CompletedTask;
}
