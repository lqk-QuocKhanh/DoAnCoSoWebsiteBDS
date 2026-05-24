using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Chat.DTOs;

public sealed class MessageDto
{
    public Guid Id { get; init; }
    public Guid ConversationId { get; init; }
    public string SenderId { get; init; } = string.Empty;
    public string? SenderName { get; init; }
    public string? SenderAvatarUrl { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? AttachmentUrl { get; init; }
    public MessageStatus Status { get; init; }
    public DateTime? ReadAt { get; init; }
    public DateTime SentAt { get; init; }

    /// <summary>True when this message was sent by the requesting user.</summary>
    public bool IsOwn { get; init; }
}
