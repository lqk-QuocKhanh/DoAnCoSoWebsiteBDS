using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.Models;

// ── User Management ───────────────────────────────────────────────────────────

public sealed class AdminUserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? PhoneNumber { get; init; }
    public string? AvatarUrl { get; init; }
    public IList<string> Roles { get; init; } = [];
    public bool IsActive { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool IsLockedOut { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public int ListingCount { get; init; }
}

public sealed class AdminUserStats
{
    public int Total { get; init; }
    public int Active { get; init; }
    public int Locked { get; init; }
    public int NewThisMonth { get; init; }
    public int Admins { get; init; }
    public int Brokers { get; init; }
    public int Customers { get; init; }
}

// ── Payment / Revenue ─────────────────────────────────────────────────────────

public sealed class AdminPaymentDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string? UserName { get; init; }
    public string? UserEmail { get; init; }
    public string? PackageName { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "VND";
    public PaymentStatus Status { get; init; }
    public string? PaymentMethod { get; init; }
    public string? TransactionCode { get; init; }
    public string? BankCode { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? PaidAt { get; init; }
}

public sealed class RevenueStats
{
    public decimal TotalRevenue { get; init; }
    public decimal MonthRevenue { get; init; }
    public decimal WeekRevenue { get; init; }
    public decimal TodayRevenue { get; init; }
    public int TotalTransactions { get; init; }
    public int SuccessRate { get; init; }
    public double[] MonthlyRevenue { get; init; } = [];
    public string[] MonthLabels { get; init; } = [];
    public double[] DailyTransactions { get; init; } = [];
    public string[] DayLabels { get; init; } = [];
}

// ── VIP Package Subscription ──────────────────────────────────────────────────

public sealed class AdminVipSubscriptionDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string? UserName { get; init; }
    public string? UserEmail { get; init; }
    public string PackageName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int RemainingListings { get; init; }
    public bool IsActive { get; init; }
    public int DaysRemaining { get; init; }
}

public sealed class VipPackageStats
{
    public string PackageName { get; init; } = string.Empty;
    public int SubscriberCount { get; init; }
    public decimal TotalRevenue { get; init; }
    public int ActiveCount { get; init; }
}

// ── Activity Log ──────────────────────────────────────────────────────────────

public sealed class AdminActivityLogDto
{
    public Guid Id { get; init; }
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public string Action { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
    public string? EntityId { get; init; }
    public string? Description { get; init; }
    public string? IpAddress { get; init; }
    public ActivityLogLevel Level { get; init; }
    public DateTime Timestamp { get; init; }
}

public enum ActivityLogLevel { Info, Warning, Error, Success }

// ── Dashboard Overview ────────────────────────────────────────────────────────

public sealed class AdminDashboardStats
{
    public int TotalUsers { get; init; }
    public int NewUsersThisMonth { get; init; }
    public int TotalListings { get; init; }
    public int PendingApprovals { get; init; }
    public int ActiveListings { get; init; }
    public decimal MonthRevenue { get; init; }
    public decimal TotalRevenue { get; init; }
    public int TotalPayments { get; init; }
    public int OnlineUsers { get; init; }
    public double[] RevenueChart { get; init; } = [];
    public double[] UsersChart { get; init; } = [];
    public string[] ChartLabels { get; init; } = [];
    public double[] PropertyTypeDistribution { get; init; } = [];
    public string[] PropertyTypeLabels { get; init; } = [];
}
