using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Notifications.DTOs;

public sealed class NotificationDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public NotificationType Type { get; init; }
    public bool IsRead { get; init; }
    public DateTime? ReadAt { get; init; }
    public string? ReferenceId { get; init; }
    public string? ActionUrl { get; init; }
    public DateTime CreatedAt { get; init; }
}
