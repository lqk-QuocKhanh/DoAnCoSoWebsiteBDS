namespace VietPropEstate.Application.Common.Interfaces;

public interface IUserPhoneVerificationService
{
    Task EnsureCanPostListingAsync(CancellationToken cancellationToken = default);
}
