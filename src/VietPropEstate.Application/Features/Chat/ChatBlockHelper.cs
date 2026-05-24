using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Chat;

internal static class ChatBlockHelper
{
    public static Task<bool> IsBlockedBetweenAsync(
        IApplicationDbContext db,
        string userA,
        string userB,
        CancellationToken cancellationToken) =>
        db.UserBlocks.AnyAsync(b =>
            (b.BlockerId == userA && b.BlockedUserId == userB) ||
            (b.BlockerId == userB && b.BlockedUserId == userA),
            cancellationToken);

    public static Task<bool> HasBlockedAsync(
        IApplicationDbContext db,
        string blockerId,
        string blockedUserId,
        CancellationToken cancellationToken) =>
        db.UserBlocks.AnyAsync(b =>
            b.BlockerId == blockerId && b.BlockedUserId == blockedUserId,
            cancellationToken);
}
