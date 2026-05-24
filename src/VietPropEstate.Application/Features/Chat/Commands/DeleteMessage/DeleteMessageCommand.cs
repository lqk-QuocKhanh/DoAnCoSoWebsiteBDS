using MediatR;

namespace VietPropEstate.Application.Features.Chat.Commands.DeleteMessage;

public sealed record DeleteMessageCommand(Guid MessageId, string RequestingUserId) : IRequest;
