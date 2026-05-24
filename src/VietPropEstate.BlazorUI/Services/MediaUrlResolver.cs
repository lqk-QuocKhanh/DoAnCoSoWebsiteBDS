namespace VietPropEstate.BlazorUI.Services;

public interface IMediaUrlResolver
{
    string? Resolve(string? url);
}

public sealed class MediaUrlResolver : IMediaUrlResolver
{
    private readonly HttpClient _http;

    public MediaUrlResolver(HttpClient http) => _http = http;

    public string? Resolve(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return url;

        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            || url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return url;

        return new Uri(_http.BaseAddress!, url.TrimStart('/')).ToString();
    }
}
