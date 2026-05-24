using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.MarkPropertyAsSold;

public sealed class MarkPropertyAsSoldCommandHandler : IRequestHandler<MarkPropertyAsSoldCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public MarkPropertyAsSoldCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(MarkPropertyAsSoldCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.Id);

        property.MarkAsSold();
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
