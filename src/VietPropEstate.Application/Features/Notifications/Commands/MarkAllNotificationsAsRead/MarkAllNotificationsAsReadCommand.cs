using MediatR;

namespace VietPropEstate.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead;

public sealed record MarkAllNotificationsAsReadCommand(string UserId) : IRequest;
