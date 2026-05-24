using VietPropEstate.Application.Features.Notifications.DTOs;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Common.Interfaces;

/// <summary>Creates and persists in-app notifications, then pushes them via SignalR.</summary>
public interface INotificationService
{
    Task<NotificationDto> CreateAndPushAsync(
        string recipientUserId,
        string title,
        string content,
        NotificationType type,
        string? referenceId = null,
        string? actionUrl = null,
        CancellationToken cancellationToken = default);
}
