namespace VietPropEstate.Application.Common.Authorization;

/// <summary>Central role-based business rules for VietPropEstate.</summary>
public static class AppRoles
{
    public const string SysAdmin = "SysAdmin";
    public const string Admin = "Admin";
    public const string Staff = "Staff";
    public const string Broker = "Broker";
    public const string Customer = "Customer";

    public static readonly string[] All = [SysAdmin, Admin, Staff, Broker, Customer];
    public static readonly string[] SelfRegisterable = [Customer, Broker];
    public static readonly string[] AssignableByAdmin = [Staff, Broker, Customer];
    public static readonly string[] AssignableBySysAdmin = All;
}

public static class RolePermissions
{
    public static bool CanSelfRegister(string role) =>
        AppRoles.SelfRegisterable.Contains(role, StringComparer.OrdinalIgnoreCase);

    public static bool CanPostListings(IEnumerable<string> roles)
    {
        var set = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return set.Contains(AppRoles.Customer) || set.Contains(AppRoles.Broker);
    }

    public static bool CanBuyVipPackage(IEnumerable<string> roles) =>
        roles.Contains(AppRoles.Broker, StringComparer.OrdinalIgnoreCase);

    public static bool CanModerateListings(IEnumerable<string> roles)
    {
        var set = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return set.Contains(AppRoles.Staff) ||
               set.Contains(AppRoles.Admin) ||
               set.Contains(AppRoles.SysAdmin);
    }

    public static bool CanManageUsers(IEnumerable<string> roles)
    {
        var set = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return set.Contains(AppRoles.Admin) || set.Contains(AppRoles.SysAdmin);
    }

    /// <summary>SysAdmin-only: create, update roles, lock/unlock any account.</summary>
    public static bool CanManageAllAccounts(IEnumerable<string> roles) =>
        roles.Contains(AppRoles.SysAdmin, StringComparer.OrdinalIgnoreCase);

    public static bool CanAccessAdminPanel(IEnumerable<string> roles) =>
        CanModerateListings(roles);

    public static bool CanManageSystem(IEnumerable<string> roles) =>
        roles.Contains(AppRoles.SysAdmin, StringComparer.OrdinalIgnoreCase);

    public static bool RequiresPhoneVerificationToPost(IEnumerable<string> roles)
    {
        var set = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return set.Contains(AppRoles.Customer) && !set.Contains(AppRoles.Broker);
    }

    public static bool CanAssignRole(IEnumerable<string> actorRoles, string targetRole)
    {
        var actor = actorRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (actor.Contains(AppRoles.SysAdmin))
            return AppRoles.All.Contains(targetRole, StringComparer.OrdinalIgnoreCase);

        if (actor.Contains(AppRoles.Admin))
            return AppRoles.AssignableByAdmin.Contains(targetRole, StringComparer.OrdinalIgnoreCase);

        return false;
    }

    public static string GetDisplayName(string role) => role switch
    {
        AppRoles.SysAdmin => "Quản trị hệ thống",
        AppRoles.Admin => "Quản trị viên",
        AppRoles.Staff => "Nhân viên kiểm duyệt",
        AppRoles.Broker => "Môi giới / Chủ BĐS",
        AppRoles.Customer => "Khách hàng",
        _ => role
    };
}
