using System.Text.RegularExpressions;
using VietPropEstate.Application.Features.Notifications.DTOs;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.Helpers;

public static partial class NotificationNavigation
{
    private static readonly Regex LegacyConversationPath = ConversationPathRegex();

    public static string? GetUrl(NotificationDto notification)
    {
        if (notification.Type == NotificationType.MessageReceived)
        {
            var conversationId = TryGetConversationId(notification);
            if (conversationId.HasValue)
                return $"/dashboard/tin-nhan?conversation={conversationId.Value}";
        }

        return notification.ActionUrl;
    }

    private static Guid? TryGetConversationId(NotificationDto notification)
    {
        if (Guid.TryParse(notification.ReferenceId, out var fromReference))
            return fromReference;

        if (string.IsNullOrEmpty(notification.ActionUrl))
            return null;

        var match = LegacyConversationPath.Match(notification.ActionUrl);
        return match.Success && Guid.TryParse(match.Groups[1].Value, out var fromActionUrl)
            ? fromActionUrl
            : null;
    }

    [GeneratedRegex(@"/conversations/([0-9a-fA-F-]{36})", RegexOptions.IgnoreCase)]
    private static partial Regex ConversationPathRegex();
}
