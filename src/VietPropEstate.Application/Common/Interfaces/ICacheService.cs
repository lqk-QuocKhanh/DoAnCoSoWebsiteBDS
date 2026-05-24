namespace VietPropEstate.Application.Common.Interfaces;

public interface ICacheService
{
    /// <summary>Gets a cached value, or returns default if not found.</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>Sets a value with an absolute expiry.</summary>
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null, CancellationToken cancellationToken = default);

    /// <summary>Removes a key from the cache.</summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Removes all keys matching a prefix pattern.</summary>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    /// <summary>Returns the cached value if found, otherwise executes the factory and caches the result.</summary>
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? absoluteExpiry = null,
        CancellationToken cancellationToken = default);

    /// <summary>Returns true if the key exists in the cache.</summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
