namespace VietPropEstate.Application.Common.Interfaces;

public interface ITokenService
{
    /// <summary>Generates a JWT access token and returns (token string, expiry UTC).</summary>
    (string Token, DateTime ExpiresAt) GenerateAccessToken(
        string userId, string email, IEnumerable<string> roles, bool emailConfirmed = false);

    /// <summary>Validates a JWT token; returns the userId claim or null if invalid/expired.</summary>
    string? ValidateAccessToken(string token);

    /// <summary>Extracts the userId from an expired JWT (used during refresh token rotation).</summary>
    string? GetUserIdFromExpiredToken(string token);

    /// <summary>Generates a cryptographically secure opaque refresh token value.</summary>
    string GenerateRefreshTokenValue();

    /// <summary>Returns the configured refresh token expiry UTC timestamp.</summary>
    DateTime GetRefreshTokenExpiry();
}
