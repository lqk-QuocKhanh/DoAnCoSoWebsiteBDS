using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VietPropEstate.BlazorUI.Models;
using VietPropEstate.BlazorUI.States;

namespace VietPropEstate.BlazorUI.Services;

public interface IAuthApiClient
{
    Task InitializeAsync();
    Task<AuthLoginResult> LoginAsync(LoginRequest request, bool rememberMe = false);
    Task<AuthLoginResult> RegisterAsync(RegisterRequest request, bool rememberMe = false);
    Task LogoutAsync();
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword, string confirmPassword);
    Task<bool> VerifyEmailAsync(string userId, string token);
    Task<UserProfile?> GetProfileAsync();
    Task<(bool Success, string? Message, UserProfile? Profile)> UpdateProfileAsync(UpdateProfileRequest request);
    Task<(bool Success, string? Message, UserProfile? Profile)> UploadAvatarAsync(
        Stream content, string fileName, string contentType);
    Task<(bool Success, string? Message)> ChangePasswordAsync(ChangePasswordRequest request);
    Task<(bool Success, string? Message, string? DevCode)> SendPhoneVerificationCodeAsync(string phoneNumber);
    Task<(bool Success, string? Message, UserProfile? Profile)> VerifyPhoneNumberAsync(string phoneNumber, string code);
}

public sealed class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _http;
    private readonly AuthState _authState;
    private readonly IAuthTokenStorage _tokenStorage;
    private readonly JwtAuthenticationStateProvider _authStateProvider;
    private readonly IMediaUrlResolver _mediaUrls;
    private readonly ILogger<AuthApiClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthApiClient(
        HttpClient http,
        AuthState authState,
        IAuthTokenStorage tokenStorage,
        JwtAuthenticationStateProvider authStateProvider,
        IMediaUrlResolver mediaUrls,
        ILogger<AuthApiClient> logger)
    {
        _http = http;
        _authState = authState;
        _tokenStorage = tokenStorage;
        _authStateProvider = authStateProvider;
        _mediaUrls = mediaUrls;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var stored = await _tokenStorage.LoadAsync();
        if (stored is null || string.IsNullOrWhiteSpace(stored.AccessToken))
            return;

        var auth = NormalizeAuth(stored);
        _authState.SetUser(auth);
        _authStateProvider.NotifyUserChanged(auth);
        _logger.LogInformation("Restored authenticated session for {Email}.", stored.Email);

        var profile = await GetProfileAsync();
        if (profile is not null)
            await SyncAuthStateAsync(profile);
    }

    public async Task<AuthLoginResult> LoginAsync(LoginRequest request, bool rememberMe = false)
    {
        try
        {
            _logger.LogInformation("Login attempt for {Email}.", request.Email);

            var resp = await _http.PostAsJsonAsync("api/auth/login", request);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                var message = TryReadErrorMessage(body)
                    ?? "Email hoặc mật khẩu không đúng. Vui lòng thử lại.";
                _logger.LogWarning("Login failed for {Email}: {Status} {Message}", request.Email, resp.StatusCode, message);
                return new AuthLoginResult { Success = false, Message = message };
            }

            var apiResponse = JsonSerializer.Deserialize<LoginApiResponse>(body, JsonOptions);
            if (apiResponse is null || !apiResponse.Success || string.IsNullOrWhiteSpace(apiResponse.Token))
            {
                _logger.LogWarning("Login response invalid for {Email}.", request.Email);
                return new AuthLoginResult
                {
                    Success = false,
                    Message = apiResponse?.Message ?? "Có lỗi xảy ra, vui lòng thử lại."
                };
            }

            var auth = NormalizeAuth(MapToAuthResponse(apiResponse));
            await PersistSessionAsync(auth, rememberMe);
            _logger.LogInformation("Login succeeded for {Email}.", request.Email);

            return new AuthLoginResult
            {
                Success = true,
                Message = apiResponse.Message ?? "Đăng nhập thành công",
                User = auth
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login request failed for {Email}.", request.Email);
            return new AuthLoginResult
            {
                Success = false,
                Message = "Có lỗi xảy ra, vui lòng thử lại. Kiểm tra kết nối API."
            };
        }
    }

    public async Task<AuthLoginResult> RegisterAsync(RegisterRequest request, bool rememberMe = false)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/auth/register", request);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return new AuthLoginResult
                {
                    Success = false,
                    Message = TryReadErrorMessage(body) ?? "Đăng ký thất bại. Vui lòng thử lại."
                };
            }

            var auth = JsonSerializer.Deserialize<AuthResponse>(body, JsonOptions);
            if (auth is null || string.IsNullOrWhiteSpace(auth.AccessToken))
            {
                return new AuthLoginResult { Success = false, Message = "Đăng ký thất bại. Vui lòng thử lại." };
            }

            await PersistSessionAsync(NormalizeAuth(auth), rememberMe);
            return new AuthLoginResult { Success = true, Message = "Đăng ký thành công", User = NormalizeAuth(auth) };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Register request failed for {Email}.", request.Email);
            return new AuthLoginResult { Success = false, Message = "Có lỗi xảy ra, vui lòng thử lại." };
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            if (_authState.IsAuthenticated)
                await _http.PostAsync("api/auth/logout", null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Logout API call failed.");
        }
        finally
        {
            await _tokenStorage.ClearAsync();
            _authState.Logout();
            _authStateProvider.NotifyUserChanged(null);
        }
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/auth/forgot-password", new { Email = email });
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, string confirmPassword)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/auth/reset-password",
                new { Email = email, Token = token, NewPassword = newPassword, ConfirmNewPassword = confirmPassword });
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> VerifyEmailAsync(string userId, string token)
    {
        try
        {
            var resp = await _http.GetAsync(
                $"api/auth/verify-email?userId={Uri.EscapeDataString(userId)}&token={Uri.EscapeDataString(token)}");
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<UserProfile?> GetProfileAsync()
    {
        try
        {
            var profile = await _http.GetFromJsonAsync<UserProfile>("api/auth/me", JsonOptions);
            return NormalizeProfile(profile);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load user profile.");
            return null;
        }
    }

    public async Task<(bool Success, string? Message, UserProfile? Profile)> UpdateProfileAsync(
        UpdateProfileRequest request)
    {
        try
        {
            var resp = await _http.PutAsJsonAsync("api/auth/me", request);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                return (false, TryReadErrorMessage(body) ?? "Không thể cập nhật hồ sơ.", null);

            var profile = JsonSerializer.Deserialize<UserProfile>(body, JsonOptions);
            profile = NormalizeProfile(profile);
            if (profile is not null)
                await SyncAuthStateAsync(profile);

            return (true, "Cập nhật hồ sơ thành công.", profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update profile failed.");
            return (false, "Có lỗi xảy ra, vui lòng thử lại.", null);
        }
    }

    public async Task<(bool Success, string? Message, UserProfile? Profile)> UploadAvatarAsync(
        Stream content, string fileName, string contentType)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StreamContent(content), "file", fileName);

            var resp = await _http.PostAsync("api/auth/me/avatar", form);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                return (false, TryReadErrorMessage(body) ?? "Không thể tải ảnh đại diện.", null);

            var profile = JsonSerializer.Deserialize<UserProfile>(body, JsonOptions);
            profile = NormalizeProfile(profile);
            if (profile is not null)
                await SyncAuthStateAsync(profile);

            return (true, "Cập nhật ảnh đại diện thành công.", profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload avatar failed.");
            return (false, "Có lỗi xảy ra, vui lòng thử lại.", null);
        }
    }

    public async Task<(bool Success, string? Message)> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/auth/change-password", request);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                return (false, TryReadErrorMessage(body) ?? "Không thể đổi mật khẩu.");

            await _tokenStorage.ClearAsync();
            _authState.Logout();
            _authStateProvider.NotifyUserChanged(null);
            return (true, "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Change password failed.");
            return (false, "Có lỗi xảy ra, vui lòng thử lại.");
        }
    }

    public async Task<(bool Success, string? Message, string? DevCode)> SendPhoneVerificationCodeAsync(string phoneNumber)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/auth/phone/send-code", new { PhoneNumber = phoneNumber });
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                return (false, TryReadErrorMessage(body) ?? "Không thể gửi mã xác thực.", null);

            var result = JsonSerializer.Deserialize<SendPhoneVerificationResponse>(body, JsonOptions);
            return (true, result?.Message ?? "Đã gửi mã xác thực.", result?.DevCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Send phone verification code failed.");
            return (false, "Có lỗi xảy ra, vui lòng thử lại.", null);
        }
    }

    public async Task<(bool Success, string? Message, UserProfile? Profile)> VerifyPhoneNumberAsync(
        string phoneNumber, string code)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("api/auth/phone/verify",
                new { PhoneNumber = phoneNumber, Code = code });
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                return (false, TryReadErrorMessage(body) ?? "Mã xác thực không đúng.", null);

            var profile = JsonSerializer.Deserialize<UserProfile>(body, JsonOptions);
            profile = NormalizeProfile(profile);
            if (profile is not null)
                await SyncAuthStateAsync(profile);

            return (true, "Xác thực số điện thoại thành công.", profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Verify phone number failed.");
            return (false, "Có lỗi xảy ra, vui lòng thử lại.", null);
        }
    }

    private async Task SyncAuthStateAsync(UserProfile profile)
    {
        var current = _authState.CurrentUser;
        if (current is null)
            return;

        var updated = new AuthResponse
        {
            AccessToken = current.AccessToken,
            RefreshToken = current.RefreshToken,
            AccessTokenExpiresAt = current.AccessTokenExpiresAt,
            UserId = current.UserId,
            Email = current.Email,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            FullName = profile.FullName,
            AvatarUrl = profile.AvatarUrl ?? _mediaUrls.Resolve(current.AvatarUrl),
            EmailConfirmed = profile.EmailConfirmed,
            Roles = profile.Roles
        };

        _authState.SetUser(updated);
        await _tokenStorage.UpdateAsync(updated);
        _authStateProvider.NotifyUserChanged(updated);
    }

    private async Task PersistSessionAsync(AuthResponse auth, bool rememberMe)
    {
        _authState.SetUser(auth);
        await _tokenStorage.SaveAsync(auth, rememberMe);
        _authStateProvider.NotifyUserChanged(auth);
    }

    private AuthResponse NormalizeAuth(AuthResponse auth)
    {
        auth.AvatarUrl = _mediaUrls.Resolve(auth.AvatarUrl);
        return auth;
    }

    private static AuthResponse MapToAuthResponse(LoginApiResponse apiResponse)
    {
        var user = apiResponse.User ?? new AuthUserInfo();
        return new AuthResponse
        {
            AccessToken = apiResponse.Token,
            RefreshToken = apiResponse.RefreshToken,
            AccessTokenExpiresAt = user.AccessTokenExpiresAt,
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            EmailConfirmed = user.EmailConfirmed,
            Roles = user.Roles
        };
    }

    private UserProfile? NormalizeProfile(UserProfile? profile)
    {
        if (profile is null || string.IsNullOrWhiteSpace(profile.AvatarUrl))
            return profile;

        profile.AvatarUrl = _mediaUrls.Resolve(profile.AvatarUrl);
        return profile;
    }

    private static string? TryReadErrorMessage(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (root.TryGetProperty("message", out var messageProp))
                return messageProp.GetString();

            if (root.TryGetProperty("errors", out var errorsProp))
            {
                if (errorsProp.ValueKind == JsonValueKind.Object)
                {
                    if (errorsProp.TryGetProperty("message", out var nestedMessage))
                        return nestedMessage.GetString();

                    foreach (var property in errorsProp.EnumerateObject())
                    {
                        if (property.Value.ValueKind == JsonValueKind.Array && property.Value.GetArrayLength() > 0)
                            return property.Value[0].GetString();
                    }
                }
            }
        }
        catch
        {
            // ignore parse errors
        }

        return null;
    }
}
