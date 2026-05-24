using MediatR;

namespace VietPropEstate.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(Guid NotificationId, string UserId) : IRequest;
