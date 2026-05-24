namespace VietPropEstate.Application.Features.Dashboard.DTOs;

public sealed class DashboardStatsDto
{
    public int TotalListings { get; init; }
    public int ActiveListings { get; init; }
    public int PendingListings { get; init; }
    public long TotalViews { get; init; }
    public int TotalFavorites { get; init; }
    public int UnreadMessages { get; init; }
    public int UnreadNotifications { get; init; }
}
