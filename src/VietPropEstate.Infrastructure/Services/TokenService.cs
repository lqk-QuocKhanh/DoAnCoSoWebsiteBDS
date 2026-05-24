using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public (string Token, DateTime ExpiresAt) GenerateAccessToken(
        string userId, string email, IEnumerable<string> roles, bool emailConfirmed = false)
    {
        var settings = GetJwtSettings();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.Add(new Claim("email_verified", emailConfirmed ? "true" : "false"));

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    /// <inheritdoc/>
    public string? ValidateAccessToken(string token)
    {
        var settings = GetJwtSettings();
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(settings.Secret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public string? GetUserIdFromExpiredToken(string token)
    {
        var settings = GetJwtSettings();
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(settings.Secret);

        try
        {
            // Validate signature but allow expired tokens
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false   // ← key difference
            }, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public string GenerateRefreshTokenValue()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <inheritdoc/>
    public DateTime GetRefreshTokenExpiry()
    {
        var days = int.Parse(
            _configuration["JwtSettings:RefreshTokenExpiryDays"] ?? "7");
        return DateTime.UtcNow.AddDays(days);
    }

    // ─── Private ─────────────────────────────────────────────────────────────

    private JwtSettings GetJwtSettings()
    {
        var section = _configuration.GetSection("JwtSettings");
        return new JwtSettings
        {
            Secret = section["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured."),
            Issuer = section["Issuer"] ?? "VietPropEstate",
            Audience = section["Audience"] ?? "VietPropEstate",
            ExpiryMinutes = int.Parse(section["ExpiryMinutes"] ?? "60")
        };
    }

    private sealed record JwtSettings
    {
        public string Secret { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int ExpiryMinutes { get; init; } = 60;
    }
}
