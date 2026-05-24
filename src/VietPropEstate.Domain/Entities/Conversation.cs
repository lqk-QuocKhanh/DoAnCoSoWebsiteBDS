using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

/// <summary>A messaging thread between a buyer and a seller about a specific property.</summary>
public class Conversation : AuditableEntity
{
    public Guid PropertyId { get; private set; }

    /// <summary>Identity user ID of the buyer/enquirer.</summary>
    public string BuyerId { get; private set; } = string.Empty;

    /// <summary>Identity user ID of the seller / agent / landlord.</summary>
    public string SellerId { get; private set; } = string.Empty;

    public bool IsClosed { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? Subject { get; private set; }

    /// <summary>When the most recent message was sent — used for sorting.</summary>
    public DateTime? LastMessageAt { get; private set; }

    /// <summary>Short preview of the last message (≤150 chars) for conversation list.</summary>
    public string? LastMessagePreview { get; private set; }

    /// <summary>Cached unread count for the buyer (updated on send/read).</summary>
    public int BuyerUnreadCount { get; private set; }

    /// <summary>Cached unread count for the seller (updated on send/read).</summary>
    public int SellerUnreadCount { get; private set; }

    public Property Property { get; private set; } = null!;

    private readonly List<Message> _messages = [];
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    private Conversation() { }

    public static Conversation Create(
        Guid propertyId,
        string buyerId,
        string sellerId,
        string? subject = null)
    {
        if (propertyId == Guid.Empty)
            throw new DomainException("Property ID is required for a conversation.");
        if (string.IsNullOrWhiteSpace(buyerId))
            throw new DomainException("Buyer ID is required for a conversation.");
        if (string.IsNullOrWhiteSpace(sellerId))
            throw new DomainException("Seller ID is required for a conversation.");
        if (buyerId == sellerId)
            throw new DomainException("Buyer and seller cannot be the same user.");

        return new Conversation
        {
            PropertyId = propertyId,
            BuyerId = buyerId,
            SellerId = sellerId,
            Subject = subject,
            IsClosed = false
        };
    }

    public void Close()
    {
        if (IsClosed)
            throw new DomainException("Conversation is already closed.");

        IsClosed = true;
        ClosedAt = DateTime.UtcNow;
    }

    public void Reopen()
    {
        if (!IsClosed)
            throw new DomainException("Conversation is not closed.");

        IsClosed = false;
        ClosedAt = null;
    }

    public bool IsParticipant(string userId) =>
        BuyerId == userId || SellerId == userId;

    public string GetOtherParticipantId(string userId) =>
        BuyerId == userId ? SellerId : BuyerId;

    public void UpdateLastMessage(string preview, string senderId)
    {
        LastMessageAt = DateTime.UtcNow;
        LastMessagePreview = preview.Length > 150 ? preview[..150] : preview;

        // Increment unread count for the OTHER participant
        if (senderId == BuyerId)
            SellerUnreadCount++;
        else
            BuyerUnreadCount++;
    }

    public void ResetUnreadCount(string userId)
    {
        if (userId == BuyerId)
            BuyerUnreadCount = 0;
        else
            SellerUnreadCount = 0;
    }

    public int GetUnreadCount(string userId) =>
        userId == BuyerId ? BuyerUnreadCount : SellerUnreadCount;
}
