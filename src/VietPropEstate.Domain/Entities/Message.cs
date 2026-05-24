using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

/// <summary>A single message within a conversation thread.</summary>
public class Message : AuditableEntity
{
    public Guid ConversationId { get; private set; }

    /// <summary>Identity user ID of the sender.</summary>
    public string SenderId { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;
    public MessageStatus Status { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public string? AttachmentUrl { get; private set; }

    public Conversation Conversation { get; private set; } = null!;

    private Message() { }

    public static Message Create(
        Guid conversationId,
        string senderId,
        string content,
        string? attachmentUrl = null)
    {
        if (conversationId == Guid.Empty)
            throw new DomainException("Conversation ID is required.");
        if (string.IsNullOrWhiteSpace(senderId))
            throw new DomainException("Sender ID is required.");
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Message content cannot be empty.");
        if (content.Length > 4000)
            throw new DomainException("Message content cannot exceed 4000 characters.");

        return new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content.Trim(),
            AttachmentUrl = attachmentUrl,
            Status = MessageStatus.Sent
        };
    }

    public void MarkAsDelivered()
    {
        if (Status == MessageStatus.Sent)
            Status = MessageStatus.Delivered;
    }

    public void MarkAsRead()
    {
        if (Status is MessageStatus.Sent or MessageStatus.Delivered)
        {
            Status = MessageStatus.Read;
            ReadAt = DateTime.UtcNow;
        }
    }

    public void SoftDelete()
    {
        Status = MessageStatus.Deleted;
        Content = "[Message deleted]";
    }
}
