using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.RemovePropertyImage;

public sealed class RemovePropertyImageCommandHandler : IRequestHandler<RemovePropertyImageCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public RemovePropertyImageCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(RemovePropertyImageCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdWithDetailsAsync(
            request.PropertyId, cancellationToken: cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.PropertyId);

        property.RemoveImage(request.ImageId);
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
