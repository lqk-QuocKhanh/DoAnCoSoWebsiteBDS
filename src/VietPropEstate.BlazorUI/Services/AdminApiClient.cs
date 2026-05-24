using System.Net.Http.Json;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.BlazorUI.Models;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.Services;

public interface IAdminApiClient
{
    Task<AdminDashboardStats> GetDashboardStatsAsync();
    Task<List<AdminUserDto>> GetUsersAsync(string? search = null, string? role = null);
    Task<AdminUserStats> GetUserStatsAsync();
    Task<bool> LockUserAsync(string userId);
    Task<bool> UnlockUserAsync(string userId);
    Task<bool> UpdateUserRoleAsync(string userId, string role);
    Task<List<AdminPaymentDto>> GetPaymentsAsync(PaymentStatus? status = null, int page = 1);
    Task<RevenueStats> GetRevenueStatsAsync();
    Task<List<AdminVipSubscriptionDto>> GetVipSubscriptionsAsync();
    Task<List<VipPackageStats>> GetVipPackageStatsAsync();
    Task<List<AdminActivityLogDto>> GetActivityLogsAsync(string? action = null, ActivityLogLevel? level = null, string? search = null);
    Task<bool> ApprovePropertyAsync(Guid propertyId);
    Task<bool> RejectPropertyAsync(Guid propertyId, string reason);
    Task<bool> HidePropertyAsync(Guid propertyId);
    Task<bool> RestorePropertyAsync(Guid propertyId);
    Task<bool> DeletePropertyAsync(Guid propertyId);
    Task<PaginatedList<PropertyDto>?> GetAdminPropertiesAsync(
        int pageNumber = 1,
        int pageSize = 50,
        PropertyStatus? status = null,
        string? searchTerm = null);
}

public sealed class AdminApiClient : IAdminApiClient
{
    private readonly HttpClient _http;

    public AdminApiClient(HttpClient http) => _http = http;

    public async Task<AdminDashboardStats> GetDashboardStatsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<AdminDashboardStats>("api/admin/stats")
                ?? new AdminDashboardStats();
        }
        catch
        {
            return new AdminDashboardStats();
        }
    }

    public async Task<List<AdminUserDto>> GetUsersAsync(string? search = null, string? role = null)
    {
        try
        {
            var url = "api/admin/users";
            var query = new List<string>();
            if (!string.IsNullOrEmpty(search))
                query.Add($"search={Uri.EscapeDataString(search)}");
            if (!string.IsNullOrEmpty(role))
                query.Add($"role={Uri.EscapeDataString(role)}");
            if (query.Count > 0)
                url += "?" + string.Join("&", query);

            return await _http.GetFromJsonAsync<List<AdminUserDto>>(url) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<AdminUserStats> GetUserStatsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<AdminUserStats>("api/admin/users/stats")
                ?? new AdminUserStats();
        }
        catch
        {
            return new AdminUserStats();
        }
    }

    public async Task<bool> LockUserAsync(string userId)
    {
        try { return (await _http.PostAsync($"api/admin/users/{userId}/lock", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> UnlockUserAsync(string userId)
    {
        try { return (await _http.PostAsync($"api/admin/users/{userId}/unlock", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> UpdateUserRoleAsync(string userId, string role)
    {
        try { return (await _http.PostAsJsonAsync($"api/admin/users/{userId}/role", new { Role = role })).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<List<AdminPaymentDto>> GetPaymentsAsync(PaymentStatus? status = null, int page = 1)
    {
        try
        {
            var url = $"api/admin/payments?pageNumber={page}&pageSize=20";
            if (status.HasValue)
                url += $"&status={(int)status.Value}";
            return await _http.GetFromJsonAsync<List<AdminPaymentDto>>(url) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<RevenueStats> GetRevenueStatsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<RevenueStats>("api/admin/revenue")
                ?? new RevenueStats();
        }
        catch
        {
            return new RevenueStats();
        }
    }

    public async Task<List<AdminVipSubscriptionDto>> GetVipSubscriptionsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<AdminVipSubscriptionDto>>("api/admin/vip-subscriptions") ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<List<VipPackageStats>> GetVipPackageStatsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<VipPackageStats>>("api/admin/vip-stats") ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<List<AdminActivityLogDto>> GetActivityLogsAsync(
        string? action = null, ActivityLogLevel? level = null, string? search = null)
    {
        try
        {
            var url = "api/admin/activity-logs";
            var query = new List<string>();
            if (!string.IsNullOrEmpty(action))
                query.Add($"action={Uri.EscapeDataString(action)}");
            if (!string.IsNullOrEmpty(search))
                query.Add($"search={Uri.EscapeDataString(search)}");
            if (query.Count > 0)
                url += "?" + string.Join("&", query);

            return await _http.GetFromJsonAsync<List<AdminActivityLogDto>>(url) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<bool> ApprovePropertyAsync(Guid propertyId)
    {
        try { return (await _http.PostAsync($"api/properties/{propertyId}/publish", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> RejectPropertyAsync(Guid propertyId, string reason)
    {
        try
        {
            return (await _http.PostAsJsonAsync($"api/admin/properties/{propertyId}/reject", new { Reason = reason }))
                .IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> HidePropertyAsync(Guid propertyId)
    {
        try { return (await _http.PostAsync($"api/properties/{propertyId}/withdraw", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> RestorePropertyAsync(Guid propertyId)
    {
        try { return (await _http.PostAsync($"api/properties/{propertyId}/restore", null)).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<bool> DeletePropertyAsync(Guid propertyId)
    {
        try { return (await _http.DeleteAsync($"api/properties/{propertyId}")).IsSuccessStatusCode; }
        catch { return false; }
    }

    public async Task<PaginatedList<PropertyDto>?> GetAdminPropertiesAsync(
        int pageNumber = 1,
        int pageSize = 50,
        PropertyStatus? status = null,
        string? searchTerm = null)
    {
        try
        {
            var url = $"api/admin/properties?pageNumber={pageNumber}&pageSize={pageSize}";
            if (status.HasValue)
                url += $"&status={(int)status.Value}";
            if (!string.IsNullOrWhiteSpace(searchTerm))
                url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";

            var result = await _http.GetFromJsonAsync<PaginatedList<PropertyDto>>(url);
            if (result is null)
                return null;

            foreach (var item in result.Items)
            {
                if (string.IsNullOrWhiteSpace(item.PrimaryImageUrl))
                {
                    item.PrimaryImageUrl = null;
                    continue;
                }

                if (!item.PrimaryImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    item.PrimaryImageUrl = new Uri(_http.BaseAddress!, item.PrimaryImageUrl.TrimStart('/')).ToString();
                }
            }

            return result;
        }
        catch
        {
            return null;
        }
    }
}
