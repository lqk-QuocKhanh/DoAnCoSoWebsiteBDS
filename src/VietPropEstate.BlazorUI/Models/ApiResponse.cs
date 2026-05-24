namespace VietPropEstate.BlazorUI.Models;

public sealed class ApiResponse<T>
{
    public bool Succeeded { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
}

public sealed class AddressProvince
{
    public int Code { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Codename { get; init; } = string.Empty;
    public string DivisionType { get; init; } = string.Empty;
}

public sealed class AddressWard
{
    public int Code { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Codename { get; init; } = string.Empty;
    public string DivisionType { get; init; } = string.Empty;
    public int ProvinceCode { get; init; }
}

public sealed class AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string? RefreshToken { get; init; }
    public DateTime AccessTokenExpiresAt { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool EmailConfirmed { get; init; }
    public IList<string> Roles { get; init; } = [];
}

public sealed class UserProfile
{
    public string UserId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? AvatarUrl { get; set; }
    public string? AddressLine { get; init; }
    public string? ProvinceName { get; init; }
    public string? WardName { get; init; }
    public int? ProvinceCode { get; init; }
    public int? WardCode { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool PhoneNumberConfirmed { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public IList<string> Roles { get; init; } = [];
}

public sealed class UpdateProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AddressLine { get; set; }
    public string? ProvinceName { get; set; }
    public string? WardName { get; set; }
    public int? ProvinceCode { get; set; }
    public int? WardCode { get; set; }
}

public sealed class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

public sealed class LoginApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public AuthUserInfo? User { get; init; }
}

public sealed class AuthUserInfo
{
    public string UserId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool EmailConfirmed { get; init; }
    public IList<string> Roles { get; init; } = [];
    public DateTime AccessTokenExpiresAt { get; init; }
}

public sealed class AuthLoginResult
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public AuthResponse? User { get; init; }
}

public sealed class LoginRequest
{
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập email")]
    [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    public string Password { get; set; } = string.Empty;
}

public sealed class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = "Customer";
}

public sealed class SendPhoneVerificationResponse
{
    public string Message { get; init; } = string.Empty;
    public string? DevCode { get; init; }
}

public sealed class DashboardStats
{
    public int TotalListings { get; init; }
    public int ActiveListings { get; init; }
    public int PendingListings { get; init; }
    public long TotalViews { get; init; }
    public int TotalFavorites { get; init; }
    public int UnreadMessages { get; init; }
    public int UnreadNotifications { get; init; }
}
