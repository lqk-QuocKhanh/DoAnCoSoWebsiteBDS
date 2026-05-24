namespace VietPropEstate.Application.Common.Interfaces;

/// <summary>
/// Tracks which users are currently connected to the SignalR hub.
/// In-memory for single-node deployments; replace with Redis for horizontal scaling.
/// </summary>
public interface IOnlineUserTracker
{
    /// <summary>Registers a new connection for a user.</summary>
    Task UserConnectedAsync(string userId, string connectionId);

    /// <summary>Removes a connection. Returns true if the user is now fully offline.</summary>
    Task<bool> UserDisconnectedAsync(string userId, string connectionId);

    /// <summary>Returns true if the user has at least one active connection.</summary>
    bool IsOnline(string userId);

    /// <summary>Returns all active connection IDs for a user.</summary>
    IReadOnlyList<string> GetConnectionIds(string userId);

    /// <summary>Returns the IDs of all currently online users.</summary>
    IReadOnlyList<string> GetOnlineUsers();

    /// <summary>Updates the "last seen" timestamp when a user disconnects.</summary>
    Task SetLastSeenAsync(string userId, DateTime timestamp);

    /// <summary>Returns the last seen timestamp for a user (null if currently online).</summary>
    Task<DateTime?> GetLastSeenAsync(string userId);
}
