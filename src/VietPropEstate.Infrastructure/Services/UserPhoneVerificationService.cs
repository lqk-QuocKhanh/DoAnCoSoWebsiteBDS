using Microsoft.AspNetCore.Identity;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Authorization;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Infrastructure.Identity;

namespace VietPropEstate.Infrastructure.Services;

public sealed class UserPhoneVerificationService : IUserPhoneVerificationService
{
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserPhoneVerificationService(
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager)
    {
        _currentUser = currentUser;
        _userManager = userManager;
    }

    public async Task EnsureCanPostListingAsync(CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsInRole(AppRoles.Customer))
            return;

        if (_currentUser.IsInRole(AppRoles.Broker))
            return;

        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
            throw new ForbiddenAccessException();

        var user = await _userManager.FindByIdAsync(_currentUser.UserId)
            ?? throw new ForbiddenAccessException();

        if (user.PhoneNumberConfirmed)
            return;

        throw new ForbiddenAccessException(
            "Bạn cần xác thực số điện thoại trước khi đăng tin.",
            ForbiddenAccessException.PhoneVerificationRequiredCode);
    }
}
