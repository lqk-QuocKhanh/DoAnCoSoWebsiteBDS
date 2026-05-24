using VietPropEstate.Application.Common.Authorization;
using VietPropEstate.Infrastructure.Identity;

namespace VietPropEstate.WebAPI.Authorization;

internal static class AdminBusinessRules
{
    public static bool IsSysAdmin(IList<string> roles) =>
        roles.Contains(AppRoles.SysAdmin, StringComparer.OrdinalIgnoreCase);

    public static bool IsAdminAccount(IList<string> roles) =>
        roles.Contains(AppRoles.Admin, StringComparer.OrdinalIgnoreCase) ||
        roles.Contains(AppRoles.SysAdmin, StringComparer.OrdinalIgnoreCase);

    public static bool CanManageUser(ApplicationUser actor, IList<string> actorRoles, ApplicationUser target, IList<string> targetRoles)
    {
        if (IsSysAdmin(targetRoles))
            return IsSysAdmin(actorRoles);

        if (string.Equals(actor.Id, target.Id, StringComparison.Ordinal))
            return false;

        if (IsAdminAccount(targetRoles) && !IsSysAdmin(actorRoles))
            return false;

        return RolePermissions.CanManageAllAccounts(actorRoles);
    }

    public static bool CanAssignRole(IList<string> actorRoles, string newRole) =>
        RolePermissions.CanAssignRole(actorRoles, newRole);
}
