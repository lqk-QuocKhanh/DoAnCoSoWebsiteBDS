using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.RestoreProperty;

public sealed class RestorePropertyCommandHandler : IRequestHandler<RestorePropertyCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public RestorePropertyCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(RestorePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.Id);

        property.RestoreFromHidden();

        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
