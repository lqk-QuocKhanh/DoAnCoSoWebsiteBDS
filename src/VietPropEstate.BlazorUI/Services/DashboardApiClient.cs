using System.Net.Http.Json;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.BlazorUI.Models;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.Services;

public interface IDashboardApiClient
{
    Task<DashboardStats> GetStatsAsync();
    Task<PaginatedList<PropertyDto>?> GetMyListingsAsync(int page = 1, PropertyStatus? status = null);
    Task<bool> DeleteListingAsync(Guid propertyId);
    Task<PaginatedList<PropertyDto>?> GetFavoritesAsync(int page = 1);
}

public sealed class DashboardApiClient : IDashboardApiClient
{
    private readonly HttpClient _http;

    public DashboardApiClient(HttpClient http) => _http = http;

    public async Task<DashboardStats> GetStatsAsync()
    {
        try
        {
            var stats = await _http.GetFromJsonAsync<DashboardStats>("api/dashboard/stats");
            if (stats is not null)
                return stats;
        }
        catch
        {
            // fall through to derived stats
        }

        var listings = await GetMyListingsAsync(1, null);
        var items = listings?.Items ?? [];
        return new DashboardStats
        {
            TotalListings = listings?.TotalCount ?? 0,
            ActiveListings = items.Count(p => p.Status == PropertyStatus.Active),
            PendingListings = items.Count(p => p.Status == PropertyStatus.PendingApproval),
            TotalViews = items.Sum(p => p.ViewCount),
            TotalFavorites = 0,
            UnreadMessages = 0,
            UnreadNotifications = 0
        };
    }

    public async Task<PaginatedList<PropertyDto>?> GetMyListingsAsync(int page = 1, PropertyStatus? status = null)
    {
        try
        {
            var url = $"api/properties/my-listings?pageNumber={page}&pageSize=10";
            if (status.HasValue)
                url += $"&status={(int)status.Value}";

            var result = await _http.GetFromJsonAsync<PaginatedList<PropertyDto>>(url);
            NormalizeImageUrls(result);
            return result;
        }
        catch
        {
            return null;
        }
    }

    private void NormalizeImageUrls(PaginatedList<PropertyDto>? result)
    {
        if (result is null)
            return;

        foreach (var item in result.Items)
        {
            if (string.IsNullOrWhiteSpace(item.PrimaryImageUrl))
            {
                item.PrimaryImageUrl = null;
                continue;
            }

            if (!item.PrimaryImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                item.PrimaryImageUrl = new Uri(_http.BaseAddress!, item.PrimaryImageUrl.TrimStart('/')).ToString();
        }
    }

    public async Task<bool> DeleteListingAsync(Guid propertyId)
    {
        try
        {
            var resp = await _http.DeleteAsync($"api/properties/{propertyId}");
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<PaginatedList<PropertyDto>?> GetFavoritesAsync(int page = 1)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<PaginatedList<FavoritePropertyDto>>(
                $"api/favorites?pageNumber={page}&pageSize=12");
            if (result is null)
                return null;

            var properties = result.Items.Select(f => f.Property).ToList();
            var mapped = PaginatedList<PropertyDto>.Create(
                properties, result.TotalCount, result.PageNumber, result.PageSize);
            NormalizeImageUrls(mapped);
            return mapped;
        }
        catch
        {
            return null;
        }
    }
}
