using System.Text.Json;
using Microsoft.JSInterop;
using VietPropEstate.BlazorUI.Models;

namespace VietPropEstate.BlazorUI.Services;

public interface IAuthTokenStorage
{
    Task SaveAsync(AuthResponse auth, bool rememberMe);
    Task UpdateAsync(AuthResponse auth);
    Task<AuthResponse?> LoadAsync();
    Task ClearAsync();
}

public sealed class AuthTokenStorage : IAuthTokenStorage
{
    private readonly IJSRuntime _js;

    public AuthTokenStorage(IJSRuntime js) => _js = js;

    public async Task SaveAsync(AuthResponse auth, bool rememberMe)
    {
        var json = JsonSerializer.Serialize(auth);
        await _js.InvokeVoidAsync("authStorage.set", json, rememberMe);
    }

    public async Task UpdateAsync(AuthResponse auth)
    {
        var rememberMe = await _js.InvokeAsync<bool>("authStorage.useRememberMe");
        await SaveAsync(auth, rememberMe);
    }

    public async Task<AuthResponse?> LoadAsync()
    {
        var json = await _js.InvokeAsync<string?>("authStorage.get");
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<AuthResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            await ClearAsync();
            return null;
        }
    }

    public async Task ClearAsync()
    {
        await _js.InvokeVoidAsync("authStorage.clear");
    }
}
