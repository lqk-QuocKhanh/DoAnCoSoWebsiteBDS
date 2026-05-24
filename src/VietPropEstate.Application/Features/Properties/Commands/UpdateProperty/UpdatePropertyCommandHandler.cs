using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Interfaces;
using VietPropEstate.Domain.ValueObjects;

namespace VietPropEstate.Application.Features.Properties.Commands.UpdateProperty;

public sealed class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePropertyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.Id);

        var price = new Money(request.Price, request.Currency);

        property.UpdateDetails(
            request.Title,
            request.Description,
            price,
            request.Area,
            request.NumberOfBedrooms,
            request.NumberOfBathrooms,
            request.NumberOfFloors,
            request.Direction,
            request.VideoUrl);

        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
