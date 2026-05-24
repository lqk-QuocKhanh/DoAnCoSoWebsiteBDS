using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.Models;

public static class AdminMockData
{
    // ── Users ─────────────────────────────────────────────────────────────────

    public static readonly List<AdminUserDto> Users =
    [
        new AdminUserDto { Id = "u1", Email = "admin@vietpropestate.vn", FirstName = "System", LastName = "Admin", Roles = ["Admin"], IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddYears(-2), LastLoginAt = DateTime.UtcNow.AddHours(-1), ListingCount = 0 },
        new AdminUserDto { Id = "u2", Email = "nguyen.minh@gmail.com", FirstName = "Nguyễn Văn", LastName = "Minh", PhoneNumber = "0912345678", Roles = ["Broker"], IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-8), LastLoginAt = DateTime.UtcNow.AddDays(-2), ListingCount = 24 },
        new AdminUserDto { Id = "u3", Email = "tran.hoa@gmail.com", FirstName = "Trần Thị", LastName = "Hoa", PhoneNumber = "0987654321", Roles = ["Broker"], IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-6), LastLoginAt = DateTime.UtcNow.AddHours(-5), ListingCount = 18 },
        new AdminUserDto { Id = "u4", Email = "le.duc@gmail.com", FirstName = "Lê Quốc", LastName = "Đức", PhoneNumber = "0905123456", Roles = ["Customer"], IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-3), LastLoginAt = DateTime.UtcNow.AddDays(-1), ListingCount = 0 },
        new AdminUserDto { Id = "u5", Email = "pham.tung@gmail.com", FirstName = "Phạm Thanh", LastName = "Tùng", PhoneNumber = "0903789012", Roles = ["Broker"], IsActive = false, EmailConfirmed = true, IsLockedOut = true, CreatedAt = DateTime.UtcNow.AddMonths(-4), LastLoginAt = DateTime.UtcNow.AddDays(-30), ListingCount = 7 },
        new AdminUserDto { Id = "u6", Email = "vo.duc@gmail.com", FirstName = "Võ Minh", LastName = "Đức", PhoneNumber = "0907456123", Roles = ["Staff"], IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-12), LastLoginAt = DateTime.UtcNow.AddHours(-3), ListingCount = 0 },
        new AdminUserDto { Id = "u7", Email = "ngo.huong@gmail.com", FirstName = "Ngô Thu", LastName = "Hương", PhoneNumber = "0911234567", Roles = ["Broker"], IsActive = true, EmailConfirmed = false, CreatedAt = DateTime.UtcNow.AddDays(-5), LastLoginAt = null, ListingCount = 0 },
        new AdminUserDto { Id = "u8", Email = "hoang.nam@gmail.com", FirstName = "Hoàng Văn", LastName = "Nam", PhoneNumber = "0916789345", Roles = ["Customer"], IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddDays(-2), LastLoginAt = DateTime.UtcNow.AddHours(-12), ListingCount = 0 },
        new AdminUserDto { Id = "u9", Email = "bui.lan@gmail.com", FirstName = "Bùi Thị", LastName = "Lan", PhoneNumber = "0919876543", Roles = ["Customer"], IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-1), LastLoginAt = DateTime.UtcNow.AddDays(-3), ListingCount = 0 },
        new AdminUserDto { Id = "u10", Email = "trinh.khoa@gmail.com", FirstName = "Trịnh Minh", LastName = "Khoa", PhoneNumber = "0908765432", Roles = ["Broker"], IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-5), LastLoginAt = DateTime.UtcNow.AddHours(-8), ListingCount = 11 },
    ];

    public static readonly AdminUserStats UserStats = new()
    {
        Total = 2847, Active = 2601, Locked = 42, NewThisMonth = 126,
        Admins = 3, Brokers = 412, Customers = 2432
    };

    // ── Payments ──────────────────────────────────────────────────────────────

    public static readonly List<AdminPaymentDto> Payments =
    [
        new AdminPaymentDto { Id = Guid.NewGuid(), UserId = "u2", UserName = "Nguyễn Văn Minh", UserEmail = "nguyen.minh@gmail.com", PackageName = "Gold", Amount = 5_000_000, Status = PaymentStatus.Completed, PaymentMethod = "VNPay", TransactionCode = "VNP24051901", BankCode = "VCB", CreatedAt = DateTime.UtcNow.AddHours(-2), PaidAt = DateTime.UtcNow.AddHours(-2) },
        new AdminPaymentDto { Id = Guid.NewGuid(), UserId = "u3", UserName = "Trần Thị Hoa", UserEmail = "tran.hoa@gmail.com", PackageName = "Cao Cấp", Amount = 2_500_000, Status = PaymentStatus.Completed, PaymentMethod = "VNPay", TransactionCode = "VNP24051902", BankCode = "TCB", CreatedAt = DateTime.UtcNow.AddHours(-5), PaidAt = DateTime.UtcNow.AddHours(-5) },
        new AdminPaymentDto { Id = Guid.NewGuid(), UserId = "u10", UserName = "Trịnh Minh Khoa", UserEmail = "trinh.khoa@gmail.com", PackageName = "Tiêu Chuẩn", Amount = 1_000_000, Status = PaymentStatus.Pending, PaymentMethod = "VNPay", TransactionCode = "VNP24051903", CreatedAt = DateTime.UtcNow.AddHours(-8) },
        new AdminPaymentDto { Id = Guid.NewGuid(), UserId = "u7", UserName = "Ngô Thu Hương", UserEmail = "ngo.huong@gmail.com", PackageName = "Cơ Bản", Amount = 500_000, Status = PaymentStatus.Failed, PaymentMethod = "VNPay", TransactionCode = "VNP24051904", BankCode = "MB", CreatedAt = DateTime.UtcNow.AddHours(-10) },
        new AdminPaymentDto { Id = Guid.NewGuid(), UserId = "u2", UserName = "Nguyễn Văn Minh", UserEmail = "nguyen.minh@gmail.com", PackageName = "Gold", Amount = 5_000_000, Status = PaymentStatus.Completed, PaymentMethod = "VNPay", TransactionCode = "VNP24051801", BankCode = "VCB", CreatedAt = DateTime.UtcNow.AddDays(-1), PaidAt = DateTime.UtcNow.AddDays(-1) },
        new AdminPaymentDto { Id = Guid.NewGuid(), UserId = "u3", UserName = "Trần Thị Hoa", UserEmail = "tran.hoa@gmail.com", PackageName = "Tiêu Chuẩn", Amount = 1_000_000, Status = PaymentStatus.Refunded, PaymentMethod = "VNPay", TransactionCode = "VNP24051701", BankCode = "ACB", CreatedAt = DateTime.UtcNow.AddDays(-2), PaidAt = DateTime.UtcNow.AddDays(-2) },
        new AdminPaymentDto { Id = Guid.NewGuid(), UserId = "u10", UserName = "Trịnh Minh Khoa", UserEmail = "trinh.khoa@gmail.com", PackageName = "Cao Cấp", Amount = 2_500_000, Status = PaymentStatus.Completed, PaymentMethod = "VNPay", TransactionCode = "VNP24051601", BankCode = "BIDV", CreatedAt = DateTime.UtcNow.AddDays(-3), PaidAt = DateTime.UtcNow.AddDays(-3) },
    ];

    public static readonly RevenueStats Revenue = new()
    {
        TotalRevenue = 450_000_000,
        MonthRevenue = 45_200_000,
        WeekRevenue = 12_500_000,
        TodayRevenue = 3_500_000,
        TotalTransactions = 1247,
        SuccessRate = 94,
        MonthlyRevenue = [28.5, 32.1, 38.7, 41.2, 36.8, 45.2, 52.0, 48.3, 44.1, 49.8, 53.2, 45.2],
        MonthLabels = ["T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12"],
        DailyTransactions = [8.0, 12.0, 7.0, 15.0, 10.0, 18.0, 14.0],
        DayLabels = ["T2", "T3", "T4", "T5", "T6", "T7", "CN"],
    };

    // ── VIP Subscriptions ─────────────────────────────────────────────────────

    public static readonly List<AdminVipSubscriptionDto> VipSubscriptions =
    [
        new AdminVipSubscriptionDto { Id = Guid.NewGuid(), UserId = "u2", UserName = "Nguyễn Văn Minh", UserEmail = "nguyen.minh@gmail.com", PackageName = "Gold", StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow.AddDays(150), RemainingListings = 72, IsActive = true, DaysRemaining = 150 },
        new AdminVipSubscriptionDto { Id = Guid.NewGuid(), UserId = "u3", UserName = "Trần Thị Hoa", UserEmail = "tran.hoa@gmail.com", PackageName = "Cao Cấp", StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(80), RemainingListings = 24, IsActive = true, DaysRemaining = 80 },
        new AdminVipSubscriptionDto { Id = Guid.NewGuid(), UserId = "u10", UserName = "Trịnh Minh Khoa", UserEmail = "trinh.khoa@gmail.com", PackageName = "Tiêu Chuẩn", StartDate = DateTime.UtcNow.AddDays(-25), EndDate = DateTime.UtcNow.AddDays(5), RemainingListings = 3, IsActive = true, DaysRemaining = 5 },
        new AdminVipSubscriptionDto { Id = Guid.NewGuid(), UserId = "u7", UserName = "Ngô Thu Hương", UserEmail = "ngo.huong@gmail.com", PackageName = "Cơ Bản", StartDate = DateTime.UtcNow.AddDays(-35), EndDate = DateTime.UtcNow.AddDays(-5), RemainingListings = 0, IsActive = false, DaysRemaining = 0 },
    ];

    public static readonly List<VipPackageStats> VipStats =
    [
        new VipPackageStats { PackageName = "Cơ Bản", SubscriberCount = 312, TotalRevenue = 156_000_000, ActiveCount = 289 },
        new VipPackageStats { PackageName = "Tiêu Chuẩn", SubscriberCount = 187, TotalRevenue = 187_000_000, ActiveCount = 162 },
        new VipPackageStats { PackageName = "Cao Cấp", SubscriberCount = 96, TotalRevenue = 240_000_000, ActiveCount = 84 },
        new VipPackageStats { PackageName = "Gold", SubscriberCount = 48, TotalRevenue = 240_000_000, ActiveCount = 41 },
    ];

    // ── Activity Logs ─────────────────────────────────────────────────────────

    public static readonly List<AdminActivityLogDto> ActivityLogs =
    [
        new AdminActivityLogDto { Id = Guid.NewGuid(), UserId = "u1", UserName = "System Admin", Action = "Approve", EntityName = "Property", EntityId = "11111111-1111-1111-1111-111111111111", Description = "Duyệt tin đăng: Căn hộ cao cấp Vinhomes Central Park", IpAddress = "192.168.1.1", Level = ActivityLogLevel.Success, Timestamp = DateTime.UtcNow.AddMinutes(-5) },
        new AdminActivityLogDto { Id = Guid.NewGuid(), UserId = "u2", UserName = "Nguyễn Văn Minh", Action = "Payment", EntityName = "Payment", EntityId = "pmt-001", Description = "Thanh toán gói Gold thành công - 5.000.000đ", IpAddress = "118.69.145.23", Level = ActivityLogLevel.Success, Timestamp = DateTime.UtcNow.AddMinutes(-12) },
        new AdminActivityLogDto { Id = Guid.NewGuid(), UserId = "u5", UserName = "Phạm Thanh Tùng", Action = "Login", EntityName = "User", EntityId = "u5", Description = "Đăng nhập thất bại - sai mật khẩu", IpAddress = "203.119.56.78", Level = ActivityLogLevel.Warning, Timestamp = DateTime.UtcNow.AddMinutes(-18) },
        new AdminActivityLogDto { Id = Guid.NewGuid(), UserId = "u1", UserName = "System Admin", Action = "Lock", EntityName = "User", EntityId = "u5", Description = "Khóa tài khoản: Phạm Thanh Tùng - vi phạm chính sách", IpAddress = "192.168.1.1", Level = ActivityLogLevel.Warning, Timestamp = DateTime.UtcNow.AddMinutes(-20) },
        new AdminActivityLogDto { Id = Guid.NewGuid(), UserId = "u3", UserName = "Trần Thị Hoa", Action = "Create", EntityName = "Property", EntityId = "prop-123", Description = "Đăng tin mới: Nhà phố liên kế Times City 5 tầng", IpAddress = "113.21.78.45", Level = ActivityLogLevel.Info, Timestamp = DateTime.UtcNow.AddMinutes(-35) },
        new AdminActivityLogDto { Id = Guid.NewGuid(), UserId = null, UserName = "Hệ thống", Action = "Error", EntityName = "EmailService", Description = "Lỗi gửi email xác thực đến: new@user.com", IpAddress = "127.0.0.1", Level = ActivityLogLevel.Error, Timestamp = DateTime.UtcNow.AddHours(-1) },
        new AdminActivityLogDto { Id = Guid.NewGuid(), UserId = "u7", UserName = "Ngô Thu Hương", Action = "Register", EntityName = "User", EntityId = "u7", Description = "Đăng ký tài khoản mới (Broker)", IpAddress = "171.235.67.89", Level = ActivityLogLevel.Info, Timestamp = DateTime.UtcNow.AddHours(-2) },
        new AdminActivityLogDto { Id = Guid.NewGuid(), UserId = "u1", UserName = "System Admin", Action = "Reject", EntityName = "Property", EntityId = "prop-456", Description = "Từ chối tin đăng - nội dung vi phạm: Đất nền sai thông tin", IpAddress = "192.168.1.1", Level = ActivityLogLevel.Warning, Timestamp = DateTime.UtcNow.AddHours(-3) },
        new AdminActivityLogDto { Id = Guid.NewGuid(), UserId = "u10", UserName = "Trịnh Minh Khoa", Action = "Update", EntityName = "Property", EntityId = "prop-789", Description = "Cập nhật thông tin tin đăng: Biệt thự Ecopark", IpAddress = "222.252.31.12", Level = ActivityLogLevel.Info, Timestamp = DateTime.UtcNow.AddHours(-4) },
        new AdminActivityLogDto { Id = Guid.NewGuid(), UserId = null, UserName = "Hệ thống", Action = "Seed", EntityName = "Address", Description = "Đồng bộ dữ liệu địa chỉ Việt Nam: 34 tỉnh, 3321 phường/xã", IpAddress = "127.0.0.1", Level = ActivityLogLevel.Info, Timestamp = DateTime.UtcNow.AddHours(-8) },
    ];

    // ── Dashboard ─────────────────────────────────────────────────────────────

    public static readonly AdminDashboardStats DashboardStats = new()
    {
        TotalUsers = 2847, NewUsersThisMonth = 126,
        TotalListings = 8231, PendingApprovals = 24, ActiveListings = 7842,
        MonthRevenue = 45_200_000, TotalRevenue = 450_000_000,
        TotalPayments = 1247, OnlineUsers = 183,
        RevenueChart = [28.5, 32.1, 38.7, 41.2, 36.8, 45.2],
        UsersChart = [215.0, 248.0, 291.0, 275.0, 312.0, 126.0],
        ChartLabels = ["T7", "T8", "T9", "T10", "T11", "T12"],
        PropertyTypeDistribution = [38.0, 25.0, 18.0, 10.0, 6.0, 3.0],
        PropertyTypeLabels = ["Căn hộ", "Nhà phố", "Đất nền", "Biệt thự", "Văn phòng", "Phòng trọ"],
    };
}
