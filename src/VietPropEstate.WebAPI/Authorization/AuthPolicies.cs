using VietPropEstate.Application.Common.Authorization;

namespace VietPropEstate.WebAPI.Authorization;

/// <summary>Named authorization policy constants used across the application.</summary>
public static class AuthPolicies
{
    public const string SysAdminOnly = nameof(SysAdminOnly);
    public const string AdminOrSysAdmin = nameof(AdminOrSysAdmin);
    public const string ModeratorOrAdmin = nameof(ModeratorOrAdmin);
    public const string StaffOrAdmin = nameof(StaffOrAdmin);
    public const string BrokerOnly = nameof(BrokerOnly);
    public const string ListingPublisher = nameof(ListingPublisher);
    public const string AuthenticatedUser = nameof(AuthenticatedUser);
    public const string RequireVerifiedEmail = nameof(RequireVerifiedEmail);

    // Legacy alias kept for compatibility
    public const string AdminOnly = AdminOrSysAdmin;
    public const string BrokerOrAdmin = BrokerOnly;
    public const string AllStaff = ModeratorOrAdmin;
}

/// <summary>Re-exports application role constants for WebAPI consumers.</summary>
public static class AppRoleNames
{
    public const string SysAdmin = AppRoles.SysAdmin;
    public const string Admin = AppRoles.Admin;
    public const string Staff = AppRoles.Staff;
    public const string Broker = AppRoles.Broker;
    public const string Customer = AppRoles.Customer;
}
