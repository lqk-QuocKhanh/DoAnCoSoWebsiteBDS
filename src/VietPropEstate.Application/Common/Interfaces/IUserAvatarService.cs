namespace VietPropEstate.Application.Common.Interfaces;

public interface IUserAvatarService
{
    Task<IReadOnlyDictionary<string, string?>> GetAvatarUrlsAsync(
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default);
}
