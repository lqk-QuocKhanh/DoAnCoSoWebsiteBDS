namespace VietPropEstate.Application.Common.Interfaces;

public interface IAgentService
{
    Task<Guid?> GetAgentIdForCurrentUserAsync(CancellationToken cancellationToken = default);

    Task<Guid> GetOrCreateAgentIdForCurrentUserAsync(CancellationToken cancellationToken = default);
}
