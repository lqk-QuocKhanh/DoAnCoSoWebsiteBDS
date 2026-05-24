using Microsoft.Extensions.Configuration;
using Npgsql;

namespace VietPropEstate.Infrastructure.Persistence;

internal static class DatabaseConnectionResolver
{
    public static string? Resolve(IConfiguration configuration)
    {
        foreach (var candidate in GetCandidates(configuration))
        {
            var normalized = NormalizeConnectionString(candidate);
            if (!string.IsNullOrWhiteSpace(normalized))
                return normalized;
        }

        return null;
    }

    private static IEnumerable<string?> GetCandidates(IConfiguration configuration)
    {
        yield return Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        yield return configuration.GetConnectionString("DefaultConnection");
        yield return Environment.GetEnvironmentVariable("DATABASE_URL");
        yield return Environment.GetEnvironmentVariable("Internal_DATABASE_URL");
    }

    internal static string? NormalizeConnectionString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim().Trim('"', '\'');

        if (IsPostgresUri(trimmed))
            return ParseDatabaseUrl(trimmed);

        return trimmed;
    }

    private static bool IsPostgresUri(string value) =>
        value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
        value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase);

    /// <summary>Render/Heroku-style postgres:// or postgresql:// URI.</summary>
    private static string ParseDatabaseUrl(string databaseUrl)
    {
        if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri))
            throw new InvalidOperationException("Invalid PostgreSQL connection URL.");

        var userInfo = uri.UserInfo.Split(':', 2);
        var database = uri.AbsolutePath.TrimStart('/');
        var questionMark = database.IndexOf('?');
        if (questionMark >= 0)
            database = database[..questionMark];

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = string.IsNullOrEmpty(database) ? "postgres" : database,
            Username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty,
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            SslMode = SslMode.Require
        };

        if (!string.IsNullOrEmpty(uri.Query))
        {
            var query = uri.Query.TrimStart('?');
            foreach (var part in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var kv = part.Split('=', 2);
                if (kv.Length != 2)
                    continue;

                var key = Uri.UnescapeDataString(kv[0]);
                var val = Uri.UnescapeDataString(kv[1]);

                if (key.Equals("sslmode", StringComparison.OrdinalIgnoreCase) &&
                    Enum.TryParse<SslMode>(val, ignoreCase: true, out var sslMode))
                {
                    builder.SslMode = sslMode;
                }
            }
        }

        return builder.ConnectionString;
    }
}
