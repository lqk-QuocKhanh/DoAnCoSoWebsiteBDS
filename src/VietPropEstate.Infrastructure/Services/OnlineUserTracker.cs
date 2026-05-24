using System.Collections.Concurrent;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Infrastructure.Services;

/// <summary>
/// Thread-safe in-memory online user tracker for single-node deployments.
/// For horizontal scaling, replace with a Redis-backed implementation using
/// StackExchange.Redis HSET to store {userId → connectionIds}.
/// </summary>
public sealed class InMemoryOnlineUserTracker : IOnlineUserTracker
{
    // userId → set of active connectionIds
    private readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

    // userId → last seen UTC
    private readonly ConcurrentDictionary<string, DateTime> _lastSeen = new();

    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task UserConnectedAsync(string userId, string connectionId)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_connections.TryGetValue(userId, out var ids))
            {
                ids = [];
                _connections[userId] = ids;
            }
            ids.Add(connectionId);
            _lastSeen.TryRemove(userId, out _); // Clear last-seen when online
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> UserDisconnectedAsync(string userId, string connectionId)
    {
        await _lock.WaitAsync();
        try
        {
            if (!_connections.TryGetValue(userId, out var ids)) return true;

            ids.Remove(connectionId);

            if (ids.Count != 0) return false; // Still has other connections

            _connections.TryRemove(userId, out _);
            return true; // Fully offline
        }
        finally
        {
            _lock.Release();
        }
    }

    public bool IsOnline(string userId) =>
        _connections.TryGetValue(userId, out var ids) && ids.Count > 0;

    public IReadOnlyList<string> GetConnectionIds(string userId) =>
        _connections.TryGetValue(userId, out var ids)
            ? [.. ids]
            : [];

    public IReadOnlyList<string> GetOnlineUsers() => [.. _connections.Keys];

    public Task SetLastSeenAsync(string userId, DateTime timestamp)
    {
        _lastSeen[userId] = timestamp;
        return Task.CompletedTask;
    }

    public Task<DateTime?> GetLastSeenAsync(string userId) =>
        Task.FromResult(_lastSeen.TryGetValue(userId, out var ts) ? (DateTime?)ts : null);
}
