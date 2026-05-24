namespace VietPropEstate.WebAPI.Extensions;

public static class HttpContextExtensions
{
    public static string GetClientIpAddress(this HttpContext context, string fallback = "127.0.0.1")
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
        {
            var ip = forwarded.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(ip))
                return ip;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? fallback;
    }
}
