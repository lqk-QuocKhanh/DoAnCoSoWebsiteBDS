using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.AddPropertyImage;

public sealed class AddPropertyImageCommandHandler : IRequestHandler<AddPropertyImageCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddPropertyImageCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(AddPropertyImageCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdWithDetailsAsync(
            request.PropertyId, asNoTracking: true, cancellationToken)
            ?? throw new NotFoundException(nameof(Property), request.PropertyId);

        var displayOrder = property.Images.Count == 0
            ? 0
            : property.Images.Max(i => i.DisplayOrder) + 1;
        var isPrimary = property.Images.Count == 0;

        var image = PropertyImage.Create(
            request.PropertyId, request.Url, request.Caption,
            displayOrder, isPrimary);

        await _unitOfWork.Properties.AddPropertyImageAsync(image, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return image.Id;
    }
}
