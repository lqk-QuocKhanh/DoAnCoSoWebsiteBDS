using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Infrastructure.Identity;

namespace VietPropEstate.Infrastructure.Services;

public sealed class UserAvatarService : IUserAvatarService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public UserAvatarService(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IReadOnlyDictionary<string, string?>> GetAvatarUrlsAsync(
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default)
    {
        var ids = userIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<string, string?>();

        var users = await _userManager.Users
            .AsNoTracking()
            .Where(u => ids.Contains(u.Id))
            .Select(u => new { u.Id, u.AvatarUrl })
            .ToListAsync(cancellationToken);

        var agents = await _context.Agents
            .AsNoTracking()
            .Where(a => a.UserId != null && ids.Contains(a.UserId))
            .Select(a => new { UserId = a.UserId!, a.AvatarUrl })
            .ToListAsync(cancellationToken);

        var result = new Dictionary<string, string?>();
        foreach (var id in ids)
        {
            var userAvatar = users.FirstOrDefault(u => u.Id == id)?.AvatarUrl;
            var agentAvatar = agents.FirstOrDefault(a => a.UserId == id)?.AvatarUrl;
            result[id] = !string.IsNullOrWhiteSpace(userAvatar) ? userAvatar : agentAvatar;
        }

        return result;
    }
}
