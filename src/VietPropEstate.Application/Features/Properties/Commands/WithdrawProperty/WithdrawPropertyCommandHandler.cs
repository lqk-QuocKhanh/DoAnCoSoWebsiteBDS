using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.WithdrawProperty;

public sealed class WithdrawPropertyCommandHandler : IRequestHandler<WithdrawPropertyCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public WithdrawPropertyCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(WithdrawPropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.Id);

        property.Withdraw();
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
