using MediatR;

namespace VietPropEstate.Application.Features.Chat.Commands.BlockUser;

public sealed class BlockUserCommand : IRequest
{
    public Guid ConversationId { get; init; }
    public string UserId { get; init; } = string.Empty;
}
