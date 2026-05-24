using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Infrastructure.Caching;

/// <summary>
/// Distributed cache implementation backed by Redis via IDistributedCache.
/// Falls back gracefully when Redis is unavailable rather than crashing.
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var bytes = await _cache.GetAsync(key, cancellationToken);
            if (bytes is null) return default;

            return JsonSerializer.Deserialize<T>(bytes, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache GET failed for key '{Key}'.", key);
            return default;
        }
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
            var options = new DistributedCacheEntryOptions();

            if (absoluteExpiry.HasValue)
                options.SetAbsoluteExpiration(absoluteExpiry.Value);
            else
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30));

            await _cache.SetAsync(key, bytes, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache SET failed for key '{Key}'.", key);
        }
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache REMOVE failed for key '{Key}'.", key);
        }
    }

    /// <inheritdoc/>
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // IDistributedCache does not natively support prefix removal.
        // With StackExchange.Redis directly you can use SCAN + DEL.
        // Here we log a notice; applications that need bulk eviction should
        // inject IConnectionMultiplexer separately for Redis-specific operations.
        _logger.LogDebug("RemoveByPrefixAsync called for prefix '{Prefix}'. " +
                          "Prefix-based removal requires IConnectionMultiplexer.", prefix);
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null) return cached;

        var value = await factory(cancellationToken);
        await SetAsync(key, value, absoluteExpiry, cancellationToken);
        return value;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var bytes = await _cache.GetAsync(key, cancellationToken);
            return bytes is not null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache EXISTS check failed for key '{Key}'.", key);
            return false;
        }
    }
}
