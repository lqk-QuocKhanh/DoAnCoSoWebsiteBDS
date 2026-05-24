using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

/// <summary>Per-user preferences for a conversation (pin, mute).</summary>
public class UserConversationSetting : AuditableEntity
{
    public string UserId { get; private set; } = string.Empty;
    public Guid ConversationId { get; private set; }
    public bool IsPinned { get; private set; }
    public bool IsMuted { get; private set; }
    public DateTime? PinnedAt { get; private set; }

    public Conversation Conversation { get; private set; } = null!;

    private UserConversationSetting() { }

    public static UserConversationSetting Create(string userId, Guid conversationId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User ID is required.");
        if (conversationId == Guid.Empty)
            throw new DomainException("Conversation ID is required.");

        return new UserConversationSetting
        {
            UserId = userId,
            ConversationId = conversationId
        };
    }

    public void SetPinned(bool pinned)
    {
        IsPinned = pinned;
        PinnedAt = pinned ? DateTime.UtcNow : null;
    }

    public void SetMuted(bool muted) => IsMuted = muted;
}
