using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Chat;

internal static class ChatParticipantLookup
{
    public static async Task<IReadOnlyDictionary<string, string>> GetDisplayNamesAsync(
        IApplicationDbContext db,
        IEnumerable<string> userIds,
        CancellationToken cancellationToken)
    {
        var ids = userIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<string, string>();

        var agents = await db.Agents
            .AsNoTracking()
            .Where(a => a.UserId != null && ids.Contains(a.UserId))
            .Select(a => new { UserId = a.UserId!, a.FullName })
            .ToListAsync(cancellationToken);

        var customers = await db.Customers
            .AsNoTracking()
            .Where(c => c.UserId != null && ids.Contains(c.UserId))
            .Select(c => new { UserId = c.UserId!, c.FullName })
            .ToListAsync(cancellationToken);

        var result = new Dictionary<string, string>();
        foreach (var id in ids)
        {
            result[id] = agents.FirstOrDefault(a => a.UserId == id)?.FullName
                ?? customers.FirstOrDefault(c => c.UserId == id)?.FullName
                ?? "Người dùng";
        }

        return result;
    }
}
