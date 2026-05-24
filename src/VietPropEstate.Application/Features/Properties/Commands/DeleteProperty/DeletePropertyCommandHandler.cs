using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.DeleteProperty;

public sealed class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePropertyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.Id);

        if (!request.IsAdmin)
        {
            if (string.IsNullOrWhiteSpace(request.RequestedByUserId))
                throw new ForbiddenAccessException();

            var agent = await _unitOfWork.Agents.FirstOrDefaultAsync(
                a => a.UserId == request.RequestedByUserId && !a.IsDeleted,
                cancellationToken);

            if (agent is null || property.AgentId != agent.Id)
                throw new ForbiddenAccessException();
        }

        property.IsDeleted = true;
        property.DeletedAt = DateTime.UtcNow;
        property.DeletedBy = request.RequestedByUserId;

        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
