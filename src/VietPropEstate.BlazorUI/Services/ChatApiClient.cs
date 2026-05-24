using System.Net.Http.Json;
using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Chat.DTOs;

namespace VietPropEstate.BlazorUI.Services;

public interface IChatApiClient
{
    Task<PaginatedList<ConversationDto>?> GetConversationsAsync(int page = 1);
    Task<PaginatedList<MessageDto>?> GetMessagesAsync(Guid conversationId, int page = 1);
    Task<MessageDto?> SendMessageAsync(Guid conversationId, string content);
    Task MarkAsReadAsync(Guid conversationId);
    Task<ConversationDto?> StartConversationAsync(Guid propertyId, string sellerId, string? message = null);
    Task<ConversationDto?> UpdateSettingsAsync(Guid conversationId, bool? isPinned = null, bool? isMuted = null);
    Task BlockUserAsync(Guid conversationId);
    Task UnblockUserAsync(Guid conversationId);
}

public sealed class ChatApiClient : IChatApiClient
{
    private readonly HttpClient _http;

    public ChatApiClient(HttpClient http) => _http = http;

    public async Task<PaginatedList<ConversationDto>?> GetConversationsAsync(int page = 1)
    {
        try { return await _http.GetFromJsonAsync<PaginatedList<ConversationDto>>($"api/conversations?pageNumber={page}&pageSize=20"); }
        catch { return null; }
    }

    public async Task<PaginatedList<MessageDto>?> GetMessagesAsync(Guid conversationId, int page = 1)
    {
        try { return await _http.GetFromJsonAsync<PaginatedList<MessageDto>>($"api/conversations/{conversationId}/messages?pageNumber={page}&pageSize=50"); }
        catch { return null; }
    }

    public async Task<MessageDto?> SendMessageAsync(Guid conversationId, string content)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync($"api/conversations/{conversationId}/messages",
                new { Content = content });
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<MessageDto>();
        }
        catch { return null; }
    }

    public async Task MarkAsReadAsync(Guid conversationId)
    {
        try { await _http.PostAsync($"api/conversations/{conversationId}/read", null); }
        catch { /* ignore */ }
    }

    public async Task<ConversationDto?> StartConversationAsync(Guid propertyId, string sellerId, string? message = null)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/conversations",
                new { PropertyId = propertyId, SellerId = sellerId, InitialMessage = message });
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<ConversationDto>();
        }
        catch { return null; }
    }

    public async Task<ConversationDto?> UpdateSettingsAsync(
        Guid conversationId, bool? isPinned = null, bool? isMuted = null)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync($"api/conversations/{conversationId}/settings",
                new { IsPinned = isPinned, IsMuted = isMuted });
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<ConversationDto>();
        }
        catch { return null; }
    }

    public async Task BlockUserAsync(Guid conversationId)
    {
        try { await _http.PostAsync($"api/conversations/{conversationId}/block", null); }
        catch { /* ignore */ }
    }

    public async Task UnblockUserAsync(Guid conversationId)
    {
        try { await _http.DeleteAsync($"api/conversations/{conversationId}/block"); }
        catch { /* ignore */ }
    }
}
