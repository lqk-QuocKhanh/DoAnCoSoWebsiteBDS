using MediatR;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Chat.DTOs;

namespace VietPropEstate.Application.Features.Chat.Queries.GetConversationMessages;

public sealed record GetConversationMessagesQuery : IRequest<PaginatedList<MessageDto>>
{
    public Guid ConversationId { get; init; }
    public string RequestingUserId { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
    /// <summary>Load messages before this timestamp (for infinite scroll / cursor-based pagination).</summary>
    public DateTime? Before { get; init; }
}
