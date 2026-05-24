using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Auth.Commands.ChangePassword;
using VietPropEstate.Application.Features.Auth.Commands.ForgotPassword;
using VietPropEstate.Application.Features.Auth.Commands.LoginUser;
using VietPropEstate.Application.Features.Auth.Commands.Logout;
using VietPropEstate.Application.Features.Auth.Commands.RefreshToken;
using VietPropEstate.Application.Features.Auth.Commands.RegisterUser;
using VietPropEstate.Application.Features.Auth.Commands.ResetPassword;
using VietPropEstate.Application.Features.Auth.Commands.RevokeToken;
using VietPropEstate.Application.Features.Auth.Commands.UpdateUserProfile;
using VietPropEstate.Application.Features.Auth.Commands.VerifyEmail;
using VietPropEstate.Application.Features.Auth.Commands.SendPhoneVerificationCode;
using VietPropEstate.Application.Features.Auth.Commands.VerifyPhoneNumber;
using VietPropEstate.Application.Features.Auth.DTOs;
using VietPropEstate.WebAPI.Extensions;
using VietPropEstate.WebAPI.Models;

namespace VietPropEstate.WebAPI.Controllers;

/// <summary>Authentication and identity management endpoints.</summary>
public class AuthController : BaseApiController
{
    private readonly ICurrentUserService _currentUser;
    private readonly IWebHostEnvironment _environment;

    public AuthController(ICurrentUserService currentUser, IWebHostEnvironment environment)
    {
        _currentUser = currentUser;
        _environment = environment;
    }

    // ── POST /api/auth/register ───────────────────────────────────────────────

    /// <summary>Register a new account. Available roles: Customer, Broker.</summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var commandWithIp = command with { IpAddress = HttpContext.GetClientIpAddress() };
        var result = await Mediator.Send(commandWithIp, cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result with { RefreshToken = string.Empty }); // Never leak RT in body
    }

    // ── POST /api/auth/login ──────────────────────────────────────────────────

    /// <summary>Authenticate with email and password. Returns JWT + refresh token cookie.</summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var commandWithIp = command with { IpAddress = HttpContext.GetClientIpAddress() };
            var result = await Mediator.Send(commandWithIp, cancellationToken);

            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(AuthResponseMapper.ToLoginResponse(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return AuthResponseMapper.LoginFailure(MapLoginErrorMessage(ex.Message));
        }
    }

    // ── POST /api/auth/refresh-token ──────────────────────────────────────────

    /// <summary>Exchange a valid refresh token (from HttpOnly cookie) for a new access token.</summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized("Refresh token cookie is missing.");

        var command = new RefreshTokenCommand { RefreshToken = refreshToken, IpAddress = HttpContext.GetClientIpAddress() };
        var result = await Mediator.Send(command, cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result with { RefreshToken = string.Empty });
    }

    // ── POST /api/auth/revoke-token ───────────────────────────────────────────

    /// <summary>Revoke a specific refresh token (admin / manual revocation).</summary>
    [HttpPost("revoke-token")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken(
        [FromBody] RevokeTokenRequest request,
        CancellationToken cancellationToken)
    {
        var token = request.Token ?? Request.Cookies["refreshToken"];
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("A refresh token must be supplied in the request body or cookie.");

        var command = new RevokeTokenCommand
        {
            RefreshToken = token,
            IpAddress = HttpContext.GetClientIpAddress(),
            Reason = request.Reason
        };

        await Mediator.Send(command, cancellationToken);
        ClearRefreshTokenCookie();
        return NoContent();
    }

    // ── POST /api/auth/logout ─────────────────────────────────────────────────

    /// <summary>Log out the current user by revoking all their active refresh tokens.</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await Mediator.Send(
            new LogoutCommand { UserId = _currentUser.UserId!, IpAddress = HttpContext.GetClientIpAddress() },
            cancellationToken);

        ClearRefreshTokenCookie();
        return NoContent();
    }

    // ── POST /api/auth/forgot-password ────────────────────────────────────────

    /// <summary>Request a password reset email. Always returns 204 to prevent email enumeration.</summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    // ── POST /api/auth/reset-password ─────────────────────────────────────────

    /// <summary>Reset the password using the token sent by email.</summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    // ── GET /api/auth/verify-email ────────────────────────────────────────────

    /// <summary>Confirm an email address using the userId + token from the verification email.</summary>
    [HttpGet("verify-email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail(
        [FromQuery] string userId,
        [FromQuery] string token,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(new VerifyEmailCommand { UserId = userId, Token = token }, cancellationToken);
        return NoContent();
    }

    // ── POST /api/auth/change-password ────────────────────────────────────────

    /// <summary>Change the currently authenticated user's password.</summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand
        {
            UserId = _currentUser.UserId!,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword,
            ConfirmNewPassword = request.ConfirmNewPassword
        };

        await Mediator.Send(command, cancellationToken);
        ClearRefreshTokenCookie();
        return NoContent();
    }

    // ── GET /api/auth/me ──────────────────────────────────────────────────────

    /// <summary>Returns the authenticated user's profile.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var authService = HttpContext.RequestServices.GetRequiredService<IAuthService>();
        var profile = await authService.GetProfileAsync(_currentUser.UserId!, cancellationToken);
        return Ok(profile);
    }

    // ── PUT /api/auth/me ──────────────────────────────────────────────────────

    /// <summary>Update the authenticated user's profile.</summary>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserProfileCommand
        {
            UserId = _currentUser.UserId!,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            AddressLine = request.AddressLine,
            ProvinceName = request.ProvinceName,
            WardName = request.WardName,
            ProvinceCode = request.ProvinceCode,
            WardCode = request.WardCode
        };

        var profile = await Mediator.Send(command, cancellationToken);
        return Ok(profile);
    }

    // ── POST /api/auth/me/avatar ──────────────────────────────────────────────

    /// <summary>Upload or replace the authenticated user's avatar image.</summary>
    [HttpPost("me/avatar")]
    [Authorize]
    [RequestSizeLimit(5_242_880)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadAvatar(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(new { message = "Không có file được tải lên." });

        var extension = ResolveImageExtension(file);
        if (extension is null)
            return BadRequest(new { message = "Chỉ hỗ trợ ảnh JPG, PNG hoặc WEBP." });

        var userId = _currentUser.UserId!;
        var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var uploadDir = Path.Combine(webRoot, "uploads", "avatars", userId);
        Directory.CreateDirectory(uploadDir);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadDir, fileName);

        await using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream, cancellationToken);

        var avatarUrl = $"/uploads/avatars/{userId}/{fileName}";
        var authService = HttpContext.RequestServices.GetRequiredService<IAuthService>();
        var profile = await authService.UpdateAvatarAsync(userId, avatarUrl, cancellationToken);

        return Ok(profile);
    }

    // ── POST /api/auth/phone/send-code ────────────────────────────────────────

    /// <summary>Gửi mã OTP xác thực số điện thoại (bắt buộc trước khi Customer đăng tin).</summary>
    [HttpPost("phone/send-code")]
    [Authorize]
    [ProducesResponseType(typeof(SendPhoneVerificationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendPhoneVerificationCode(
        [FromBody] SendPhoneCodeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new SendPhoneVerificationCodeCommand
        {
            UserId = _currentUser.UserId!,
            PhoneNumber = request.PhoneNumber
        }, cancellationToken);

        return Ok(result);
    }

    // ── POST /api/auth/phone/verify ─────────────────────────────────────────────

    /// <summary>Xác thực số điện thoại bằng mã OTP.</summary>
    [HttpPost("phone/verify")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyPhoneNumber(
        [FromBody] VerifyPhoneRequest request,
        CancellationToken cancellationToken)
    {
        var profile = await Mediator.Send(new VerifyPhoneNumberCommand
        {
            UserId = _currentUser.UserId!,
            PhoneNumber = request.PhoneNumber,
            Code = request.Code
        }, cancellationToken);

        return Ok(profile);
    }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private static string? ResolveImageExtension(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension is ".jpg" or ".jpeg" or ".png" or ".webp")
            return extension == ".jpeg" ? ".jpg" : extension;

        return file.ContentType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => null
        };
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    private static string MapLoginErrorMessage(string message) => message switch
    {
        var m when m.Contains("verify your email", StringComparison.OrdinalIgnoreCase)
            => "Tài khoản chưa được xác thực. Vui lòng xác thực email trước khi đăng nhập.",
        var m when m.Contains("locked", StringComparison.OrdinalIgnoreCase)
            => "Tài khoản đã bị khóa do đăng nhập sai nhiều lần. Vui lòng thử lại sau 15 phút.",
        var m when m.Contains("deactivated", StringComparison.OrdinalIgnoreCase)
            => "Tài khoản đã bị vô hiệu hóa. Vui lòng liên hệ hỗ trợ.",
        _ => "Email hoặc mật khẩu không đúng. Vui lòng thử lại."
    };

    private void ClearRefreshTokenCookie() =>
        Response.Cookies.Delete("refreshToken");
}

// ── Inline request models (simple enough for controller-layer use) ────────────

public record RevokeTokenRequest(string? Token, string? Reason);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmNewPassword);
public record UpdateUserProfileRequest(
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? AddressLine,
    string? ProvinceName,
    string? WardName,
    int? ProvinceCode,
    int? WardCode);
public record SendPhoneCodeRequest(string PhoneNumber);
public record VerifyPhoneRequest(string PhoneNumber, string Code);
