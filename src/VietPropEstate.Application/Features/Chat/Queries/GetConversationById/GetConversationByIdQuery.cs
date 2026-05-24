using MediatR;
using VietPropEstate.Application.Features.Chat.DTOs;

namespace VietPropEstate.Application.Features.Chat.Queries.GetConversationById;

public sealed record GetConversationByIdQuery(Guid ConversationId, string RequestingUserId)
    : IRequest<ConversationDto>;
