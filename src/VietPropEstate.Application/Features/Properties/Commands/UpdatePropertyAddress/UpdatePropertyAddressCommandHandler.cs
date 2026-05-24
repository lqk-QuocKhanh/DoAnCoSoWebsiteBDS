using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.UpdatePropertyAddress;

public sealed class UpdatePropertyAddressCommandHandler : IRequestHandler<UpdatePropertyAddressCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdatePropertyAddressCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(UpdatePropertyAddressCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.PropertyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.PropertyId);

        property.UpdateVietnamAddress(
            request.ProvinceCode, request.ProvinceName,
            request.WardCode, request.WardName,
            request.Latitude, request.Longitude);

        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
