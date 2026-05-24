using MediatR;

namespace VietPropEstate.Application.Features.Notifications.Queries.GetUnreadNotificationCount;

public sealed record GetUnreadNotificationCountQuery(string UserId) : IRequest<int>;
