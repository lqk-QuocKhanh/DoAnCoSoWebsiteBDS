using MediatR;
using VietPropEstate.Application.Features.Chat.DTOs;

namespace VietPropEstate.Application.Features.Chat.Commands.SendMessage;

public sealed record SendMessageCommand : IRequest<MessageDto>
{
    public Guid ConversationId { get; init; }
    public string SenderId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? AttachmentUrl { get; init; }
}
