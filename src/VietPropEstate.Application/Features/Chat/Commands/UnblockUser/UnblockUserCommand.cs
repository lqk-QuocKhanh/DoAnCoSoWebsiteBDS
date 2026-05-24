using MediatR;

namespace VietPropEstate.Application.Features.Chat.Commands.UnblockUser;

public sealed class UnblockUserCommand : IRequest
{
    public Guid ConversationId { get; init; }
    public string UserId { get; init; } = string.Empty;
}
