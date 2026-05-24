namespace VietPropEstate.Application.Features.Chat.DTOs;

public sealed class ConversationDto
{
    public Guid Id { get; init; }
    public Guid PropertyId { get; init; }
    public string? PropertyTitle { get; init; }
    public string? PropertySlug { get; init; }
    public string? PropertyImageUrl { get; init; }

    public string BuyerId { get; init; } = string.Empty;
    public string? BuyerName { get; init; }
    public string? BuyerAvatarUrl { get; init; }

    public string SellerId { get; init; } = string.Empty;
    public string? SellerName { get; init; }
    public string? SellerAvatarUrl { get; init; }

    public string? Subject { get; init; }
    public bool IsClosed { get; init; }
    public DateTime? LastMessageAt { get; init; }
    public string? LastMessagePreview { get; init; }
    public int UnreadCount { get; init; }
    public bool IsPinned { get; init; }
    public bool IsMuted { get; init; }
    public bool IsBlockedByMe { get; init; }
    public bool IsBlockedByOther { get; init; }

    /// <summary>Whether the other participant is currently online.</summary>
    public bool OtherParticipantOnline { get; init; }
    public string? OtherParticipantId { get; init; }
    public string? OtherParticipantName { get; init; }
    public string? OtherParticipantAvatarUrl { get; init; }

    public DateTime CreatedAt { get; init; }
}
