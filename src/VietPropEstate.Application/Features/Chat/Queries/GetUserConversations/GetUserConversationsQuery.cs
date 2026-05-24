using MediatR;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Chat.DTOs;

namespace VietPropEstate.Application.Features.Chat.Queries.GetUserConversations;

public sealed record GetUserConversationsQuery : IRequest<PaginatedList<ConversationDto>>
{
    public string UserId { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool IncludeClosed { get; init; } = false;
}
