using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

/// <summary>JWT refresh token — one record per issued token.</summary>
public class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;

    /// <summary>Identity user ID that owns this token.</summary>
    public string UserId { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? CreatedByIp { get; private set; }

    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? RevokedReason { get; private set; }

    /// <summary>The replacement token issued when this one was rotated.</summary>
    public string? ReplacedByToken { get; private set; }

    public bool IsActive => !IsRevoked && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    private RefreshToken() { }

    public static RefreshToken Create(
        string token,
        string userId,
        DateTime expiresAt,
        string? createdByIp = null)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Token value is required.");
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User ID is required for a refresh token.");
        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Expiry date must be in the future.");

        return new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = createdByIp,
            IsRevoked = false
        };
    }

    public void Revoke(string? revokedByIp = null, string? reason = null, string? replacedByToken = null)
    {
        if (IsRevoked)
            throw new DomainException("Token is already revoked.");

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        RevokedReason = reason;
        ReplacedByToken = replacedByToken;
    }
}
