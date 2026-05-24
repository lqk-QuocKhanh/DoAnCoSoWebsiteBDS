using System.Net.Http.Json;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Notifications.DTOs;

namespace VietPropEstate.BlazorUI.Services;

public interface IFavoritesApiClient
{
    Task<bool> ToggleFavoriteAsync(Guid propertyId);
    Task<bool> IsFavoritedAsync(Guid propertyId);
}

public interface INotificationsApiClient
{
    Task<PaginatedList<NotificationDto>?> GetNotificationsAsync(int page = 1, bool unreadOnly = false);
    Task<int> GetUnreadCountAsync();
    Task MarkAsReadAsync(Guid id);
    Task MarkAllAsReadAsync();
    Task<(int Messages, int Notifications)> GetUnreadCountsAsync();
}

public sealed class FavoritesApiClient : IFavoritesApiClient
{
    private readonly HttpClient _http;

    public FavoritesApiClient(HttpClient http) => _http = http;

    public async Task<bool> ToggleFavoriteAsync(Guid propertyId)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync($"api/favorites/{propertyId}", new { });
            if (!resp.IsSuccessStatusCode)
                return false;

            var result = await resp.Content.ReadFromJsonAsync<ToggleFavoriteResponse>();
            return result?.Added ?? false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsFavoritedAsync(Guid propertyId)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<FavoriteStatusResponse>(
                $"api/favorites/{propertyId}/status");
            return result?.IsFavorited ?? false;
        }
        catch
        {
            return false;
        }
    }

    private sealed class ToggleFavoriteResponse
    {
        public bool Added { get; set; }
    }

    private sealed class FavoriteStatusResponse
    {
        public bool IsFavorited { get; set; }
    }
}

public sealed class NotificationsApiClient : INotificationsApiClient
{
    private readonly HttpClient _http;

    public NotificationsApiClient(HttpClient http) => _http = http;

    public async Task<PaginatedList<NotificationDto>?> GetNotificationsAsync(int page = 1, bool unreadOnly = false)
    {
        try
        {
            return await _http.GetFromJsonAsync<PaginatedList<NotificationDto>>(
                $"api/notifications?pageNumber={page}&pageSize=20&unreadOnly={unreadOnly.ToString().ToLowerInvariant()}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<int> GetUnreadCountAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<CountResponse>("api/notifications/count");
            return result?.Count ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    public async Task<(int Messages, int Notifications)> GetUnreadCountsAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<UnreadCountsResponse>("api/conversations/unread");
            return (result?.UnreadMessages ?? 0, result?.UnreadNotifications ?? 0);
        }
        catch
        {
            return (0, 0);
        }
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        try { await _http.PostAsync($"api/notifications/{id}/read", null); }
        catch { /* ignore */ }
    }

    public async Task MarkAllAsReadAsync()
    {
        try { await _http.PostAsync("api/notifications/read-all", null); }
        catch { /* ignore */ }
    }

    private sealed class CountResponse
    {
        public int Count { get; set; }
    }

    private sealed class UnreadCountsResponse
    {
        public int UnreadMessages { get; set; }
        public int UnreadNotifications { get; set; }
    }
}
