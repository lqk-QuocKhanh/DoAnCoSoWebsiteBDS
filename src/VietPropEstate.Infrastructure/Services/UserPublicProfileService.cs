using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Infrastructure.Identity;

namespace VietPropEstate.Infrastructure.Services;

public sealed class UserPublicProfileService : IUserPublicProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public UserPublicProfileService(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<UserPublicProfile?> GetProfileAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || !user.IsActive)
            return null;

        var agent = await _context.Agents
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId && !a.IsDeleted, cancellationToken);

        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive, cancellationToken);

        var displayName = agent?.FullName
            ?? customer?.FullName
            ?? user.FullName;

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = "Người dùng";

        var avatarUrl = !string.IsNullOrWhiteSpace(user.AvatarUrl)
            ? user.AvatarUrl
            : agent?.AvatarUrl;

        return new UserPublicProfile
        {
            UserId = userId,
            DisplayName = displayName,
            AvatarUrl = avatarUrl,
            Email = agent?.Email ?? customer?.Email ?? user.Email,
            PhoneNumber = agent?.PhoneNumber ?? customer?.PhoneNumber ?? user.PhoneNumber,
            MemberSince = agent?.CreatedAt ?? user.CreatedAt,
            AgentId = agent?.Id,
            AgencyName = agent?.AgencyName,
            Bio = agent?.Bio
        };
    }
}
