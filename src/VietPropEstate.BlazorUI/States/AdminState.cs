using VietPropEstate.BlazorUI.Services;

namespace VietPropEstate.BlazorUI.States;

public sealed class AdminState
{
    public int PendingApprovals { get; private set; }

    public event Action? OnChange;

    public void SetPendingApprovals(int count)
    {
        PendingApprovals = Math.Max(0, count);
        OnChange?.Invoke();
    }

    public async Task RefreshPendingApprovalsAsync(IAdminApiClient adminClient)
    {
        var stats = await adminClient.GetDashboardStatsAsync();
        SetPendingApprovals(stats.PendingApprovals);
    }
}
