using Microsoft.AspNetCore.Identity;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Interfaces;
using VietPropEstate.Infrastructure.Identity;

namespace VietPropEstate.Infrastructure.Services;

public sealed class AgentService : IAgentService
{
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public AgentService(
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid?> GetAgentIdForCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_currentUser.UserId))
            return null;

        var agent = await _unitOfWork.Agents.FirstOrDefaultAsync(
            a => a.UserId == _currentUser.UserId && a.IsActive && !a.IsDeleted,
            cancellationToken);

        return agent?.Id;
    }

    public async Task<Guid> GetOrCreateAgentIdForCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var existingId = await GetAgentIdForCurrentUserAsync(cancellationToken);
        if (existingId.HasValue)
            return existingId.Value;

        var userId = _currentUser.UserId
            ?? throw new ForbiddenAccessException();

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new ForbiddenAccessException();

        var agent = Agent.Create(
            user.FullName,
            user.Email ?? user.UserName ?? "agent@vietpropestate.vn",
            user.PhoneNumber ?? "",
            userId: userId);

        await _unitOfWork.Agents.AddAsync(agent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return agent.Id;
    }
}
