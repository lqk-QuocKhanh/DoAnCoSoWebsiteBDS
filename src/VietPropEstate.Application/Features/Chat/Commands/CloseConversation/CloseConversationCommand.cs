using MediatR;

namespace VietPropEstate.Application.Features.Chat.Commands.CloseConversation;

public sealed record CloseConversationCommand(Guid ConversationId, string RequestingUserId) : IRequest;
