using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Helpers;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Auth.Commands.UpdateUserProfile;
using VietPropEstate.Application.Features.Auth.DTOs;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Infrastructure.Identity;
using VietPropEstate.Infrastructure.Persistence;

namespace VietPropEstate.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private static readonly string[] AllowedRoles = ["Admin", "Staff", "Broker", "Customer"];

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IEmailService emailService,
        ISmsService smsService,
        ApplicationDbContext context,
        IConfiguration configuration,
        IHostEnvironment environment,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _smsService = smsService;
        _context = context;
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    // ── Register ────────────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> RegisterAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        string? phoneNumber,
        string role,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            throw new InvalidOperationException("An account with this email address already exists.");

        var normalizedRole = AllowedRoles
            .FirstOrDefault(r => r.Equals(role, StringComparison.OrdinalIgnoreCase))
            ?? "Customer";

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName?.Trim(),
            LastName = lastName?.Trim(),
            PhoneNumber = PhoneNumberHelper.Normalize(phoneNumber),
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", createResult.Errors.Select(e => e.Description)));

        var roleResult = await _userManager.AddToRoleAsync(user, normalizedRole);
        if (!roleResult.Succeeded)
            _logger.LogWarning("Failed to assign role '{Role}' to user {UserId}.", normalizedRole, user.Id);

        // Send email verification
        try
        {
            var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailService.SendEmailConfirmationAsync(
                user.Email!, user.FullName, user.Id, verificationToken, cancellationToken);

            await _emailService.SendWelcomeEmailAsync(user.Email!, user.FullName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send verification email to {Email}.", email);
        }

        var (accessToken, expiresAt) = await BuildAccessTokenAsync(user);
        var refreshToken = await CreateRefreshTokenAsync(user.Id, ipAddress, cancellationToken);

        _logger.LogInformation("User {Email} registered with role {Role}.", email, normalizedRole);

        return BuildResponse(user, accessToken, expiresAt, refreshToken,
            await _userManager.GetRolesAsync(user));
    }

    // ── Login ──────────────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> LoginAsync(
        string email,
        string password,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            _logger.LogWarning("Login failed: unknown email {Email} from {Ip}.", email, ipAddress ?? "unknown");
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Your account has been deactivated. Please contact support.");

        if (await _userManager.IsLockedOutAsync(user))
            throw new UnauthorizedAccessException("Your account has been locked. Please contact support.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            _logger.LogWarning("Login failed for {Email}: account locked out.", email);
            throw new UnauthorizedAccessException(
                "Your account has been locked due to multiple failed attempts. Try again in 15 minutes.");
        }

        if (result.IsNotAllowed)
        {
            _logger.LogWarning("Login failed for {Email}: email not confirmed.", email);
            throw new UnauthorizedAccessException(
                "Login is not allowed. Please verify your email address first.");
        }

        if (!result.Succeeded)
        {
            _logger.LogWarning("Login failed for {Email}: invalid credentials from {Ip}.", email, ipAddress ?? "unknown");
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var (accessToken, expiresAt) = await BuildAccessTokenAsync(user);
        var refreshToken = await CreateRefreshTokenAsync(user.Id, ipAddress, cancellationToken);

        _logger.LogInformation("User {Email} logged in from {Ip}.", email, ipAddress ?? "unknown");

        return BuildResponse(user, accessToken, expiresAt, refreshToken,
            await _userManager.GetRolesAsync(user));
    }

    // ── Refresh Token ──────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (!storedToken.IsActive)
        {
            if (storedToken.IsRevoked)
            {
                // Detected token reuse — revoke the entire family
                await RevokeDescendantTokensAsync(storedToken.UserId, ipAddress,
                    "Suspicious token reuse detected.", cancellationToken);
            }
            throw new UnauthorizedAccessException("Refresh token is no longer valid.");
        }

        var user = await _userManager.FindByIdAsync(storedToken.UserId)
            ?? throw new UnauthorizedAccessException("User not found.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User account is inactive.");

        if (await _userManager.IsLockedOutAsync(user))
            throw new UnauthorizedAccessException("User account is locked.");

        // Rotate: revoke current token and issue new one
        var newRefreshTokenValue = _tokenService.GenerateRefreshTokenValue();
        storedToken.Revoke(ipAddress, "Rotated", newRefreshTokenValue);

        var newRefreshToken = RefreshToken.Create(
            newRefreshTokenValue, user.Id, _tokenService.GetRefreshTokenExpiry(), ipAddress);

        _context.RefreshTokens.Update(storedToken);  // Mark as revoked
        await _context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var (accessToken, expiresAt) = await BuildAccessTokenAsync(user);

        _logger.LogInformation("Refresh token rotated for user {UserId}.", user.Id);

        return BuildResponse(user, accessToken, expiresAt, newRefreshTokenValue,
            await _userManager.GetRolesAsync(user));
    }

    // ── Revoke Token ───────────────────────────────────────────────────────────

    public async Task RevokeTokenAsync(
        string refreshToken,
        string? ipAddress,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken)
            ?? throw new NotFoundException("Refresh token not found.");

        if (!storedToken.IsActive)
            throw new InvalidOperationException("Token is already revoked or expired.");

        storedToken.Revoke(ipAddress, reason ?? "Manually revoked");
        _context.RefreshTokens.Update(storedToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token revoked for user {UserId}.", storedToken.UserId);
    }

    // ── Logout ─────────────────────────────────────────────────────────────────

    public async Task LogoutAsync(string userId, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
            token.Revoke(ipAddress, "User logout");

        if (activeTokens.Count > 0)
        {
            _context.RefreshTokens.UpdateRange(activeTokens);
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("User {UserId} logged out, {Count} token(s) revoked.", userId, activeTokens.Count);
    }

    // ── Forgot Password ────────────────────────────────────────────────────────

    public async Task ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        // Always return success to prevent user enumeration
        if (user is null || !user.IsActive)
        {
            _logger.LogInformation("Password reset requested for non-existent/inactive email: {Email}.", email);
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var baseUrl = _configuration["EmailSettings:AppBaseUrl"] ?? "https://localhost:5001";
        var encodedToken = Uri.EscapeDataString(token);
        var resetLink = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(email)}&token={encodedToken}";

        try
        {
            await _emailService.SendPasswordResetAsync(user.Email!, user.FullName, resetLink, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}.", email);
        }

        _logger.LogInformation("Password reset email sent to {Email}.", email);
    }

    // ── Reset Password ─────────────────────────────────────────────────────────

    public async Task ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new NotFoundException("User not found.");

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        // Revoke all existing refresh tokens after password reset
        await LogoutAsync(user.Id, null, cancellationToken);

        _logger.LogInformation("Password reset for user {Email}.", email);
    }

    // ── Verify Email ───────────────────────────────────────────────────────────

    public async Task VerifyEmailAsync(
        string userId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        if (user.EmailConfirmed)
            return; // Already confirmed — idempotent

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        _logger.LogInformation("Email verified for user {UserId}.", userId);
    }

    // ── Change Password ────────────────────────────────────────────────────────

    public async Task ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        // Revoke all refresh tokens after password change (force re-login)
        await LogoutAsync(userId, null, cancellationToken);

        _logger.LogInformation("Password changed for user {UserId}.", userId);
    }

    // ── Get Profile ────────────────────────────────────────────────────────────

    public async Task<UserProfileDto> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return new UserProfileDto
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            AddressLine = user.AddressLine,
            ProvinceName = user.ProvinceName,
            WardName = user.WardName,
            ProvinceCode = user.ProvinceCode,
            WardCode = user.WardCode,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = [.. roles]
        };
    }

    public async Task<UserProfileDto> UpdateProfileAsync(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId)
            ?? throw new NotFoundException("User not found.");

        user.FirstName = command.FirstName?.Trim();
        user.LastName = command.LastName?.Trim();

        var normalizedPhone = PhoneNumberHelper.Normalize(command.PhoneNumber);
        if (!string.Equals(user.PhoneNumber, normalizedPhone, StringComparison.Ordinal))
        {
            user.PhoneNumber = normalizedPhone;
            user.PhoneNumberConfirmed = false;
        }

        user.AddressLine = command.AddressLine?.Trim();
        user.ProvinceName = command.ProvinceName?.Trim();
        user.WardName = command.WardName?.Trim();
        user.ProvinceCode = command.ProvinceCode;
        user.WardCode = command.WardCode;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        _logger.LogInformation("Profile updated for user {UserId}.", user.Id);
        return await GetProfileAsync(user.Id, cancellationToken);
    }

    public async Task<UserProfileDto> UpdateAvatarAsync(
        string userId,
        string avatarUrl,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        user.AvatarUrl = avatarUrl;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        var agent = await _context.Agents
            .FirstOrDefaultAsync(a => a.UserId == userId && !a.IsDeleted, cancellationToken);

        if (agent is not null)
        {
            agent.UpdateProfile(
                agent.FullName,
                agent.PhoneNumber,
                agent.AgencyName,
                agent.Bio,
                avatarUrl);
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Avatar updated for user {UserId}.", userId);
        return await GetProfileAsync(userId, cancellationToken);
    }

    public async Task<SendPhoneVerificationResponseDto> SendPhoneVerificationCodeAsync(
        string userId,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var normalizedPhone = PhoneNumberHelper.Normalize(phoneNumber)
            ?? throw new InvalidOperationException("Số điện thoại không hợp lệ.");

        var existingUser = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.PhoneNumber == normalizedPhone &&
                     u.PhoneNumberConfirmed &&
                     u.Id != userId,
                cancellationToken);

        if (existingUser is not null)
            throw new InvalidOperationException("Số điện thoại này đã được sử dụng bởi tài khoản khác.");

        var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, normalizedPhone);
        await _smsService.SendVerificationCodeAsync(normalizedPhone, code, cancellationToken);

        _logger.LogInformation("Phone verification code sent to user {UserId}.", userId);

        var exposeDevCode = _environment.IsDevelopment() || _environment.IsEnvironment("Testing");
        return new SendPhoneVerificationResponseDto
        {
            Message = "Mã xác thực đã được gửi đến số điện thoại của bạn.",
            DevCode = exposeDevCode ? code : null
        };
    }

    public async Task<UserProfileDto> VerifyPhoneNumberAsync(
        string userId,
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var normalizedPhone = PhoneNumberHelper.Normalize(phoneNumber)
            ?? throw new InvalidOperationException("Số điện thoại không hợp lệ.");

        var existingUser = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.PhoneNumber == normalizedPhone &&
                     u.PhoneNumberConfirmed &&
                     u.Id != userId,
                cancellationToken);

        if (existingUser is not null)
            throw new InvalidOperationException("Số điện thoại này đã được sử dụng bởi tài khoản khác.");

        var result = await _userManager.ChangePhoneNumberAsync(user, normalizedPhone, code);
        if (!result.Succeeded)
            throw new InvalidOperationException("Mã xác thực không đúng hoặc đã hết hạn.");

        var agent = await _context.Agents
            .FirstOrDefaultAsync(a => a.UserId == userId && !a.IsDeleted, cancellationToken);

        if (agent is not null)
        {
            agent.UpdateProfile(
                agent.FullName,
                normalizedPhone,
                agent.AgencyName,
                agent.Bio,
                user.AvatarUrl);
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Phone number verified for user {UserId}.", userId);
        return await GetProfileAsync(userId, cancellationToken);
    }

    // ── Private helpers ────────────────────────────────────────────────────────

    private async Task<(string Token, DateTime ExpiresAt)> BuildAccessTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return _tokenService.GenerateAccessToken(user.Id, user.Email!, roles, user.EmailConfirmed);
    }

    private async Task<string> CreateRefreshTokenAsync(
        string userId,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        // Clean up old expired/revoked tokens before adding a new one (keep DB tidy)
        var staleTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && (rt.IsRevoked || rt.ExpiresAt < DateTime.UtcNow))
            .ToListAsync(cancellationToken);

        if (staleTokens.Count > 0)
            _context.RefreshTokens.RemoveRange(staleTokens);

        var tokenValue = _tokenService.GenerateRefreshTokenValue();
        var expiresAt = _tokenService.GetRefreshTokenExpiry();
        var refreshToken = RefreshToken.Create(tokenValue, userId, expiresAt, ipAddress);

        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return tokenValue;
    }

    private async Task RevokeDescendantTokensAsync(
        string userId,
        string? ipAddress,
        string reason,
        CancellationToken cancellationToken)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
            token.Revoke(ipAddress, reason);

        _context.RefreshTokens.UpdateRange(activeTokens);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static AuthResponseDto BuildResponse(
        ApplicationUser user,
        string accessToken,
        DateTime accessTokenExpiresAt,
        string refreshToken,
        IList<string> roles)
    {
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            EmailConfirmed = user.EmailConfirmed,
            Roles = roles
        };
    }
}
