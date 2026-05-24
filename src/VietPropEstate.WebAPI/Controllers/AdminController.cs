using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VietPropEstate.Application.Common.Authorization;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Properties.Commands.RejectProperty;
using VietPropEstate.Application.Features.Properties.Commands.WithdrawProperty;
using VietPropEstate.Application.Features.Properties.Queries.GetPropertiesList;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Infrastructure.Identity;
using VietPropEstate.WebAPI.Authorization;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>Administration endpoints — users, revenue, VIP, audit logs.</summary>
[Authorize]
public class AdminController : BaseApiController
{
    private readonly IApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public AdminController(
        IApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUser)
    {
        _db = db;
        _userManager = userManager;
        _currentUser = currentUser;
    }

    [HttpGet("stats")]
    [Authorize(Policy = AuthPolicies.ModeratorOrAdmin)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var users = await _userManager.Users.AsNoTracking().ToListAsync(cancellationToken);
        var newThisMonth = users.Count(u => u.CreatedAt >= monthStart);

        var properties = await _db.Properties
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .Select(p => p.Status)
            .ToListAsync(cancellationToken);

        var payments = await _db.Payments
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == PaymentStatus.Completed)
            .ToListAsync(cancellationToken);

        var monthRevenue = payments
            .Where(p => (p.PaidAt ?? p.CreatedAt) >= monthStart)
            .Sum(p => p.Amount.Amount);

        var totalRevenue = payments.Sum(p => p.Amount.Amount);

        var monthlyRevenue = new double[12];
        for (var i = 0; i < 12; i++)
        {
            var start = new DateTime(now.Year, i + 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);
            monthlyRevenue[i] = (double)payments
                .Where(p => (p.PaidAt ?? p.CreatedAt) >= start && (p.PaidAt ?? p.CreatedAt) < end)
                .Sum(p => p.Amount.Amount) / 1_000_000d;
        }

        var usersChart = new double[12];
        for (var i = 0; i < 12; i++)
        {
            var start = new DateTime(now.Year, i + 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);
            usersChart[i] = users.Count(u => u.CreatedAt >= start && u.CreatedAt < end);
        }

        var typeGroups = await _db.Properties
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .Include(p => p.PropertyType)
            .GroupBy(p => p.PropertyType!.Name)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(6)
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            TotalUsers = users.Count,
            NewUsersThisMonth = newThisMonth,
            TotalListings = properties.Count,
            PendingApprovals = properties.Count(s => s == PropertyStatus.PendingApproval),
            ActiveListings = properties.Count(s => s == PropertyStatus.Active),
            MonthRevenue = monthRevenue,
            TotalRevenue = totalRevenue,
            TotalPayments = await _db.Payments.CountAsync(p => !p.IsDeleted, cancellationToken),
            OnlineUsers = 0,
            RevenueChart = monthlyRevenue,
            UsersChart = usersChart,
            ChartLabels = Enumerable.Range(1, 12).Select(m => $"T{m}").ToArray(),
            PropertyTypeDistribution = typeGroups.Select(g => (double)g.Count).ToArray(),
            PropertyTypeLabels = typeGroups.Select(g => g.Name).ToArray()
        });
    }

    [HttpGet("properties")]
    [Authorize(Policy = AuthPolicies.ModeratorOrAdmin)]
    public async Task<IActionResult> GetProperties(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] PropertyStatus? status = null,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetPropertiesListQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Status = status,
            SearchTerm = searchTerm,
            AllStatuses = true,
            SortBy = "CreatedAt",
            SortOrder = "Desc"
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet("users")]
    [Authorize(Policy = AuthPolicies.AdminOrSysAdmin)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search,
        [FromQuery] string? role,
        CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(u =>
                (u.Email != null && u.Email.ToLower().Contains(term)) ||
                (u.FirstName != null && u.FirstName.ToLower().Contains(term)) ||
                (u.LastName != null && u.LastName.ToLower().Contains(term)));
        }

        var users = await query.OrderByDescending(u => u.CreatedAt).Take(200).ToListAsync(cancellationToken);

        var agents = await _db.Agents
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.UserId != null)
            .Select(a => new { a.UserId, a.Id })
            .ToListAsync(cancellationToken);

        var listingCounts = await _db.Properties
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .GroupBy(p => p.AgentId)
            .Select(g => new { AgentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AgentId, x => x.Count, cancellationToken);

        var agentByUser = agents.ToDictionary(a => a.UserId!, a => a.Id);

        var result = new List<object>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (!string.IsNullOrWhiteSpace(role) && !roles.Contains(role, StringComparer.OrdinalIgnoreCase))
                continue;

            var listingCount = agentByUser.TryGetValue(user.Id, out var agentId)
                ? listingCounts.GetValueOrDefault(agentId)
                : 0;

            result.Add(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.AvatarUrl,
                Roles = roles,
                user.IsActive,
                user.EmailConfirmed,
                IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
                user.CreatedAt,
                user.LastLoginAt,
                ListingCount = listingCount
            });
        }

        return Ok(result);
    }

    [HttpGet("users/stats")]
    [Authorize(Policy = AuthPolicies.AdminOrSysAdmin)]
    public async Task<IActionResult> GetUserStats(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.AsNoTracking().ToListAsync(cancellationToken);
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var admins = 0;
        var brokers = 0;
        var customers = 0;
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(AppRoles.Admin)) admins++;
            if (roles.Contains(AppRoles.Broker)) brokers++;
            if (roles.Contains(AppRoles.Customer)) customers++;
        }

        return Ok(new
        {
            Total = users.Count,
            Active = users.Count(u => !u.LockoutEnd.HasValue || u.LockoutEnd <= DateTimeOffset.UtcNow),
            Locked = users.Count(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow),
            NewThisMonth = users.Count(u => u.CreatedAt >= monthStart),
            Admins = admins,
            Brokers = brokers,
            Customers = customers
        });
    }

    [HttpPost("users/{userId}/lock")]
    [Authorize(Policy = AuthPolicies.AdminOrSysAdmin)]
    public async Task<IActionResult> LockUser(string userId)
    {
        var actor = await GetCurrentUserAsync();
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        var targetRoles = await _userManager.GetRolesAsync(user);
        var actorRoles = await _userManager.GetRolesAsync(actor);

        if (!AdminBusinessRules.CanManageUser(actor, actorRoles, user, targetRoles))
            return BadRequest(new { message = "You cannot lock this user account." });

        await _userManager.SetLockoutEnabledAsync(user, true);
        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        await _userManager.UpdateAsync(user);
        await RevokeUserRefreshTokensAsync(user.Id, "Account locked by admin");
        await WriteAuditAsync(AuditAction.Update, "User", user.Id, $"Locked user {user.Email}");
        return NoContent();
    }

    [HttpPost("users/{userId}/unlock")]
    [Authorize(Policy = AuthPolicies.AdminOrSysAdmin)]
    public async Task<IActionResult> UnlockUser(string userId)
    {
        var actor = await GetCurrentUserAsync();
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        var targetRoles = await _userManager.GetRolesAsync(user);
        var actorRoles = await _userManager.GetRolesAsync(actor);

        if (!AdminBusinessRules.CanManageUser(actor, actorRoles, user, targetRoles))
            return BadRequest(new { message = "You cannot unlock this user account." });

        await _userManager.SetLockoutEndDateAsync(user, null);
        await _userManager.UpdateAsync(user);
        await WriteAuditAsync(AuditAction.Update, "User", user.Id, $"Unlocked user {user.Email}");
        return NoContent();
    }

    [HttpPost("users/{userId}/role")]
    [Authorize(Policy = AuthPolicies.AdminOrSysAdmin)]
    public async Task<IActionResult> UpdateRole(string userId, [FromBody] UpdateRoleRequest request)
    {
        var actor = await GetCurrentUserAsync();
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        var targetRoles = await _userManager.GetRolesAsync(user);
        var actorRoles = await _userManager.GetRolesAsync(actor);

        if (!AdminBusinessRules.CanManageUser(actor, actorRoles, user, targetRoles))
            return BadRequest(new { message = "You cannot change this user's role." });

        if (AdminBusinessRules.IsAdminAccount([request.Role]) && !AdminBusinessRules.IsSysAdmin(actorRoles))
            return BadRequest(new { message = "Chỉ Quản trị hệ thống mới được gán vai trò Admin hoặc SysAdmin." });

        if (string.Equals(request.Role, AppRoles.SysAdmin, StringComparison.OrdinalIgnoreCase) &&
            !AdminBusinessRules.IsSysAdmin(actorRoles))
            return BadRequest(new { message = "Chỉ Quản trị hệ thống mới được gán vai trò SysAdmin." });

        if (!AdminBusinessRules.CanAssignRole(actorRoles, request.Role))
            return BadRequest(new { message = "Bạn không có quyền gán vai trò này." });

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, request.Role);
        await WriteAuditAsync(AuditAction.Update, "User", user.Id, $"Changed role to {request.Role}");
        return NoContent();
    }

    [HttpGet("payments")]
    [Authorize(Policy = AuthPolicies.AdminOrSysAdmin)]
    public async Task<IActionResult> GetPayments(
        [FromQuery] PaymentStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Payments.AsNoTracking().Where(p => !p.IsDeleted);
        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var payments = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.UserVIPPackage!)
                .ThenInclude(u => u.VIPPackage)
            .ToListAsync(cancellationToken);

        var userIds = payments.Select(p => p.UserId).Distinct().ToList();
        var users = await _userManager.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var result = payments.Select(p =>
        {
            users.TryGetValue(p.UserId, out var user);
            return new
            {
                p.Id,
                p.UserId,
                UserName = user?.FullName,
                UserEmail = user?.Email,
                PackageName = p.UserVIPPackage?.VIPPackage?.Name,
                Amount = p.Amount.Amount,
                Currency = p.Amount.Currency,
                p.Status,
                p.PaymentMethod,
                p.TransactionCode,
                p.BankCode,
                p.CreatedAt,
                p.PaidAt
            };
        });

        return Ok(result);
    }

    [HttpGet("revenue")]
    [Authorize(Policy = AuthPolicies.AdminOrSysAdmin)]
    public async Task<IActionResult> GetRevenue(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var weekStart = now.Date.AddDays(-(int)now.DayOfWeek + 1);
        var todayStart = now.Date;

        var payments = await _db.Payments
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .ToListAsync(cancellationToken);

        var completed = payments.Where(p => p.Status == PaymentStatus.Completed).ToList();
        decimal SumSince(DateTime since) => completed
            .Where(p => (p.PaidAt ?? p.CreatedAt) >= since)
            .Sum(p => p.Amount.Amount);

        var monthlyRevenue = new double[12];
        for (var i = 0; i < 12; i++)
        {
            var start = new DateTime(now.Year, i + 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);
            monthlyRevenue[i] = (double)completed
                .Where(p => (p.PaidAt ?? p.CreatedAt) >= start && (p.PaidAt ?? p.CreatedAt) < end)
                .Sum(p => p.Amount.Amount) / 1_000_000d;
        }

        var dailyTransactions = new double[7];
        for (var i = 0; i < 7; i++)
        {
            var day = weekStart.AddDays(i);
            dailyTransactions[i] = payments.Count(p => (p.PaidAt ?? p.CreatedAt).Date == day.Date);
        }

        var totalCount = payments.Count;
        var successCount = payments.Count(p => p.Status == PaymentStatus.Completed);

        return Ok(new
        {
            TotalRevenue = completed.Sum(p => p.Amount.Amount),
            MonthRevenue = SumSince(monthStart),
            WeekRevenue = SumSince(weekStart),
            TodayRevenue = SumSince(todayStart),
            TotalTransactions = totalCount,
            SuccessRate = totalCount == 0 ? 0 : (int)Math.Round(successCount * 100.0 / totalCount),
            MonthlyRevenue = monthlyRevenue,
            MonthLabels = Enumerable.Range(1, 12).Select(m => $"T{m}").ToArray(),
            DailyTransactions = dailyTransactions,
            DayLabels = new[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" }
        });
    }

    [HttpGet("vip-subscriptions")]
    [Authorize(Policy = AuthPolicies.AdminOrSysAdmin)]
    public async Task<IActionResult> GetVipSubscriptions(CancellationToken cancellationToken)
    {
        var subs = await _db.UserVIPPackages
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .Include(u => u.VIPPackage)
            .OrderByDescending(u => u.CreatedAt)
            .Take(100)
            .ToListAsync(cancellationToken);

        var userIds = subs.Select(s => s.UserId).Distinct().ToList();
        var users = await _userManager.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var result = subs.Select(s =>
        {
            users.TryGetValue(s.UserId, out var user);
            var daysRemaining = Math.Max(0, (int)(s.EndDate - DateTime.UtcNow).TotalDays);
            return new
            {
                s.Id,
                s.UserId,
                UserName = user?.FullName,
                UserEmail = user?.Email,
                PackageName = s.VIPPackage.Name,
                s.StartDate,
                s.EndDate,
                s.RemainingListings,
                s.IsActive,
                DaysRemaining = daysRemaining
            };
        });

        return Ok(result);
    }

    [HttpGet("vip-stats")]
    [Authorize(Policy = AuthPolicies.AdminOrSysAdmin)]
    public async Task<IActionResult> GetVipStats(CancellationToken cancellationToken)
    {
        var packages = await _db.VIPPackages
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .ToListAsync(cancellationToken);

        var subs = await _db.UserVIPPackages
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .Include(u => u.VIPPackage)
            .ToListAsync(cancellationToken);

        var payments = await _db.Payments
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Status == PaymentStatus.Completed && p.VIPPackageId != null)
            .ToListAsync(cancellationToken);

        var revenueByPackage = payments
            .GroupBy(p => p.VIPPackageId!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount.Amount));

        var result = packages.Select(pkg =>
        {
            var packageSubs = subs.Where(s => s.VIPPackageId == pkg.Id).ToList();
            var revenue = revenueByPackage.GetValueOrDefault(pkg.Id);
            return new
            {
                PackageName = pkg.Name,
                SubscriberCount = packageSubs.Count,
                TotalRevenue = revenue,
                ActiveCount = packageSubs.Count(s => s.IsActive && s.EndDate > DateTime.UtcNow)
            };
        });

        return Ok(result);
    }

    [HttpGet("activity-logs")]
    [Authorize(Policy = AuthPolicies.SysAdminOnly)]
    public async Task<IActionResult> GetActivityLogs(
        [FromQuery] string? action,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var query = _db.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(action) &&
            Enum.TryParse<AuditAction>(action, true, out var auditAction))
            query = query.Where(l => l.Action == auditAction);

        var logs = await query
            .OrderByDescending(l => l.Timestamp)
            .Take(100)
            .ToListAsync(cancellationToken);

        var userIds = logs.Where(l => l.UserId != null).Select(l => l.UserId!).Distinct().ToList();
        var users = await _userManager.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var result = logs
            .Where(l => string.IsNullOrWhiteSpace(search) ||
                        l.EntityName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        (l.EntityId != null && l.EntityId.Contains(search, StringComparison.OrdinalIgnoreCase)))
            .Select(l =>
            {
                users.TryGetValue(l.UserId ?? "", out var user);
                return new
                {
                    l.Id,
                    l.UserId,
                    UserName = user?.FullName ?? "Hệ thống",
                    Action = l.Action.ToString(),
                    l.EntityName,
                    l.EntityId,
                    Description = $"{l.Action} {l.EntityName}" + (l.EntityId != null ? $": {l.EntityId}" : ""),
                    l.IpAddress,
                    Level = MapLogLevel(l.Action),
                    l.Timestamp
                };
            });

        return Ok(result);
    }

    [HttpPost("properties/{propertyId:guid}/reject")]
    [Authorize(Policy = AuthPolicies.ModeratorOrAdmin)]
    public async Task<IActionResult> RejectProperty(
        Guid propertyId,
        [FromBody] RejectPropertyRequest request,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(new RejectPropertyCommand(propertyId), cancellationToken);
        await WriteAuditAsync(
            AuditAction.Reject,
            "Property",
            propertyId.ToString(),
            $"Rejected property. Reason: {request.Reason}");
        return NoContent();
    }

    private async Task<ApplicationUser> GetCurrentUserAsync()
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
            throw new UnauthorizedAccessException();

        return await _userManager.FindByIdAsync(_currentUser.UserId)
            ?? throw new UnauthorizedAccessException();
    }

    private async Task RevokeUserRefreshTokensAsync(string userId, string reason)
    {
        var tokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
            token.Revoke(reason: reason);

        if (tokens.Count > 0)
            await _db.SaveChangesAsync(CancellationToken.None);
    }

    private async Task WriteAuditAsync(AuditAction action, string entityName, string entityId, string description)
    {
        _db.AuditLogs.Add(AuditLog.Create(
            action,
            entityName,
            entityId,
            userId: _currentUser.UserId,
            newValues: description));
        await _db.SaveChangesAsync(CancellationToken.None);
    }

    private static int MapLogLevel(AuditAction action) => action switch
    {
        AuditAction.Login or AuditAction.Logout => 0,
        AuditAction.Delete or AuditAction.Reject => 1,
        AuditAction.Payment or AuditAction.Approve or AuditAction.Publish => 3,
        _ => 0
    };
}

public record UpdateRoleRequest(string Role);
public record RejectPropertyRequest(string Reason);
