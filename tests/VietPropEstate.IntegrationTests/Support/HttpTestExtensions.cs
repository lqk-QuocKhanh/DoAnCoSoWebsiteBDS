using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace VietPropEstate.IntegrationTests.Support;

internal static class HttpTestExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<string> LoginAndGetTokenAsync(
        this HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<LoginApiResponse>(JsonOptions)
            ?? throw new InvalidOperationException("Login response was empty.");

        if (!payload.Success || string.IsNullOrWhiteSpace(payload.Token))
            throw new InvalidOperationException($"Login failed: {payload.Message}");

        return payload.Token;
    }

    public static void SetBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private sealed class LoginApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
    }
}
