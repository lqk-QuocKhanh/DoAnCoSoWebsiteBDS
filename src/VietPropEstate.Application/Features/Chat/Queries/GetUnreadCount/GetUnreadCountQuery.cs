using MediatR;

namespace VietPropEstate.Application.Features.Chat.Queries.GetUnreadCount;

public sealed record GetUnreadCountQuery(string UserId) : IRequest<UnreadCountResult>;

public sealed record UnreadCountResult(int UnreadMessages, int UnreadNotifications);
