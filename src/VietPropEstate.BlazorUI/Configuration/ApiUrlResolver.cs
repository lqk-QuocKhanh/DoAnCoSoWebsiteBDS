namespace VietPropEstate.BlazorUI.Configuration;

using Microsoft.Extensions.Configuration;

public static class ApiUrlResolver
{
    public static Uri Resolve(IConfiguration configuration)
    {
        var baseUrl =
            configuration["ApiSettings:BaseUrl"]
            ?? configuration["ApiBaseUrl"]
            ?? configuration["API_BASE_URL"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "API base URL is not configured. Set ApiSettings:BaseUrl in appsettings.json " +
                "(local) or ApiSettings__BaseUrl / API_BASE_URL (Render Docker).");
        }

        baseUrl = baseUrl.Trim();
        if (!baseUrl.EndsWith('/'))
            baseUrl += '/';

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException($"Invalid API base URL: {baseUrl}");
        }

        return uri;
    }
}
