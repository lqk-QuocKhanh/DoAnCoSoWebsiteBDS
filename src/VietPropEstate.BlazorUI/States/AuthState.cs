using VietPropEstate.Application.Common.Authorization;
using VietPropEstate.BlazorUI.Models;

namespace VietPropEstate.BlazorUI.States;

public sealed class AuthState
{
    private AuthResponse? _currentUser;

    public AuthResponse? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser is not null;

    public IEnumerable<string> Roles => _currentUser?.Roles ?? [];

    public bool IsSysAdmin => HasRole(AppRoles.SysAdmin);
    public bool IsAdmin => HasRole(AppRoles.Admin);
    public bool IsStaff => HasRole(AppRoles.Staff);
    public bool IsBroker => HasRole(AppRoles.Broker);
    public bool IsCustomer => HasRole(AppRoles.Customer);

    public bool CanPostListings => RolePermissions.CanPostListings(Roles);
    public bool CanBuyVipPackage => RolePermissions.CanBuyVipPackage(Roles);
    public bool CanModerateListings => RolePermissions.CanModerateListings(Roles);
    public bool CanManageUsers => RolePermissions.CanManageUsers(Roles);
    public bool CanAccessAdminPanel => RolePermissions.CanAccessAdminPanel(Roles);
    public bool CanManageSystem => RolePermissions.CanManageSystem(Roles);
    public bool CanManageAllAccounts => RolePermissions.CanManageAllAccounts(Roles);
    public bool RequiresPhoneVerificationToPost => RolePermissions.RequiresPhoneVerificationToPost(Roles);

    public event Action? OnChange;

    public void SetUser(AuthResponse? user)
    {
        _currentUser = user;
        NotifyStateChanged();
    }

    public void Logout()
    {
        _currentUser = null;
        NotifyStateChanged();
    }

    private bool HasRole(string role) =>
        _currentUser?.Roles.Contains(role, StringComparer.OrdinalIgnoreCase) ?? false;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
