namespace VietPropEstate.Application.Common.Interfaces;

public sealed class UserPublicProfile
{
    public string UserId { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime MemberSince { get; init; }
    public Guid? AgentId { get; init; }
    public string? AgencyName { get; init; }
    public string? Bio { get; init; }
}

public interface IUserPublicProfileService
{
    Task<UserPublicProfile?> GetProfileAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
