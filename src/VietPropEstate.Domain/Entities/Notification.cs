using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

/// <summary>An in-app notification sent to a user.</summary>
public class Notification : AuditableEntity
{
    /// <summary>Identity user ID of the recipient.</summary>
    public string UserId { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    /// <summary>Optional ID of the related entity (property ID, payment ID, etc.).</summary>
    public string? ReferenceId { get; private set; }

    /// <summary>Optional deep-link URL to navigate the user to the relevant page.</summary>
    public string? ActionUrl { get; private set; }

    private Notification() { }

    public static Notification Create(
        string userId,
        string title,
        string content,
        NotificationType type,
        string? referenceId = null,
        string? actionUrl = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User ID is required for a notification.");
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Notification title is required.");
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Notification content is required.");

        return new Notification
        {
            UserId = userId,
            Title = title.Trim(),
            Content = content.Trim(),
            Type = type,
            IsRead = false,
            ReferenceId = referenceId,
            ActionUrl = actionUrl
        };
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }
}
