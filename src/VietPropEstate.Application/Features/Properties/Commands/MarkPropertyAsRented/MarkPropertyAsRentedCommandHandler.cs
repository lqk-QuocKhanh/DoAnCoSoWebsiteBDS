using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.MarkPropertyAsRented;

public sealed class MarkPropertyAsRentedCommandHandler : IRequestHandler<MarkPropertyAsRentedCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public MarkPropertyAsRentedCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(MarkPropertyAsRentedCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.Id);

        property.MarkAsRented();
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
