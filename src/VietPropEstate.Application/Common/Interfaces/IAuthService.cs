using VietPropEstate.Application.Features.Auth.Commands.UpdateUserProfile;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(
        string email,
        string password,
        string? firstName,
        string? lastName,
        string? phoneNumber,
        string role,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<AuthResponseDto> LoginAsync(
        string email,
        string password,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<AuthResponseDto> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task RevokeTokenAsync(
        string refreshToken,
        string? ipAddress,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(
        string userId,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task ForgotPasswordAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task VerifyEmailAsync(
        string userId,
        string token,
        CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<UserProfileDto> GetProfileAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<UserProfileDto> UpdateProfileAsync(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken = default);

    Task<UserProfileDto> UpdateAvatarAsync(
        string userId,
        string avatarUrl,
        CancellationToken cancellationToken = default);

    Task<SendPhoneVerificationResponseDto> SendPhoneVerificationCodeAsync(
        string userId,
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<UserProfileDto> VerifyPhoneNumberAsync(
        string userId,
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default);
}
