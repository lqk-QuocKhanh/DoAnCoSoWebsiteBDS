namespace VietPropEstate.Application.Common.Interfaces;

public interface IIdentityUserLookup
{
    Task<string?> FindUserIdByEmailAsync(string email, CancellationToken cancellationToken = default);
}
