using Microsoft.AspNetCore.Identity;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Infrastructure.Identity;

public sealed class IdentityUserLookup : IIdentityUserLookup
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityUserLookup(UserManager<ApplicationUser> userManager) =>
        _userManager = userManager;

    public async Task<string?> FindUserIdByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var user = await _userManager.FindByEmailAsync(email);
        return user?.Id;
    }
}
