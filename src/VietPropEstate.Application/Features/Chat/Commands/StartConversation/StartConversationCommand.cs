using MediatR;
using VietPropEstate.Application.Features.Chat.DTOs;

namespace VietPropEstate.Application.Features.Chat.Commands.StartConversation;

public sealed record StartConversationCommand : IRequest<ConversationDto>
{
    public Guid PropertyId { get; init; }
    public string BuyerId { get; init; } = string.Empty;
    public string SellerId { get; init; } = string.Empty;
    public string? Subject { get; init; }
    public string? InitialMessage { get; init; }
}
