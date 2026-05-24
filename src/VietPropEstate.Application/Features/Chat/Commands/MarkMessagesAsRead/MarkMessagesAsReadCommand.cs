using MediatR;

namespace VietPropEstate.Application.Features.Chat.Commands.MarkMessagesAsRead;

public sealed record MarkMessagesAsReadCommand : IRequest
{
    public Guid ConversationId { get; init; }
    public string UserId { get; init; } = string.Empty;
}
