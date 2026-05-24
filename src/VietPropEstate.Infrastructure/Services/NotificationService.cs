using Microsoft.Extensions.Logging;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Notifications.DTOs;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Infrastructure.Services;

/// <summary>
/// Persists in-app notifications to the database and pushes them to connected clients via SignalR.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _db;
    private readonly IChatNotificationService _chat;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IApplicationDbContext db,
        IChatNotificationService chat,
        ILogger<NotificationService> logger)
    {
        _db = db;
        _chat = chat;
        _logger = logger;
    }

    public async Task<NotificationDto> CreateAndPushAsync(
        string recipientUserId,
        string title,
        string content,
        NotificationType type,
        string? referenceId = null,
        string? actionUrl = null,
        CancellationToken cancellationToken = default)
    {
        var entity = Notification.Create(
            recipientUserId, title, content, type, referenceId, actionUrl);

        await _db.Notifications.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        var dto = new NotificationDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Title = entity.Title,
            Content = entity.Content,
            Type = entity.Type,
            IsRead = entity.IsRead,
            ReferenceId = entity.ReferenceId,
            ActionUrl = entity.ActionUrl,
            CreatedAt = entity.CreatedAt
        };

        try
        {
            await _chat.SendNotificationToUserAsync(recipientUserId, dto, cancellationToken);
        }
        catch (Exception ex)
        {
            // SignalR push failure is non-critical; notification is already persisted in DB
            _logger.LogWarning(ex, "Failed to push notification {Id} to user {UserId} via SignalR.",
                entity.Id, recipientUserId);
        }

        return dto;
    }
}
