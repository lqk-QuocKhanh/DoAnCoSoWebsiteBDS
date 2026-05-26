using MediatR;
using VietPropEstate.Application.Features.Chat.DTOs;

namespace VietPropEstate.Application.Features.Chat.Commands.StartSupportConversation;

public sealed record StartSupportConversationCommand : IRequest<ConversationDto>
{
    public string UserId { get; init; } = string.Empty;
    public string? AdminEmail { get; init; }
    public string? InitialMessage { get; init; }
}
