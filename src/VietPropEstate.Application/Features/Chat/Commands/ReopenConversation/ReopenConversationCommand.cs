using MediatR;

namespace VietPropEstate.Application.Features.Chat.Commands.ReopenConversation;

public sealed record ReopenConversationCommand(Guid ConversationId, string RequestingUserId) : IRequest;
