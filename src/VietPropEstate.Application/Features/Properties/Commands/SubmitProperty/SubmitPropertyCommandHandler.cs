using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.SubmitProperty;

public sealed class SubmitPropertyCommandHandler : IRequestHandler<SubmitPropertyCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAgentService _agentService;
    private readonly ICurrentUserService _currentUser;

    public SubmitPropertyCommandHandler(
        IUnitOfWork unitOfWork,
        IAgentService agentService,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _agentService = agentService;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(SubmitPropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.Id);

        if (!_currentUser.IsInRole("Admin"))
        {
            var agentId = await _agentService.GetOrCreateAgentIdForCurrentUserAsync(cancellationToken);
            if (property.AgentId != agentId)
                throw new ForbiddenAccessException();
        }

        property.SubmitForApproval();

        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
