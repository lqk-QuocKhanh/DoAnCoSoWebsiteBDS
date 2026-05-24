using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Chat;

internal static class ConversationParticipantHelper
{
    public static async Task<(string OtherId, string? OtherName, string? OtherAvatar)> GetOtherParticipantAsync(
        IApplicationDbContext db,
        IUserAvatarService avatarService,
        string buyerId,
        string sellerId,
        string requestingUserId,
        CancellationToken cancellationToken)
    {
        var otherId = buyerId == requestingUserId ? sellerId : buyerId;
        var names = await ChatParticipantLookup.GetDisplayNamesAsync(db, new[] { otherId }, cancellationToken);
        var avatars = await avatarService.GetAvatarUrlsAsync(new[] { otherId }, cancellationToken);

        names.TryGetValue(otherId, out var otherName);
        avatars.TryGetValue(otherId, out var otherAvatar);

        return (otherId, otherName, otherAvatar);
    }
}
