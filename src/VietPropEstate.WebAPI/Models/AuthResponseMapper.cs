using Microsoft.AspNetCore.Mvc;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.WebAPI.Models;

public static class AuthResponseMapper
{
    public static LoginApiResponse ToLoginResponse(AuthResponseDto dto) => new()
    {
        Success = true,
        Message = "Đăng nhập thành công",
        Token = dto.AccessToken,
        RefreshToken = dto.RefreshToken,
        User = new AuthUserDto
        {
            UserId = dto.UserId,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            FullName = dto.FullName,
            AvatarUrl = dto.AvatarUrl,
            EmailConfirmed = dto.EmailConfirmed,
            Roles = dto.Roles,
            AccessTokenExpiresAt = dto.AccessTokenExpiresAt
        }
    };

    public static IActionResult LoginFailure(string message, int statusCode = StatusCodes.Status401Unauthorized)
        => new ObjectResult(new LoginApiResponse
        {
            Success = false,
            Message = message
        })
        { StatusCode = statusCode };
}
