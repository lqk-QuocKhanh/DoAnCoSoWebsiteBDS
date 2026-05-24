using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.SetPropertyFeatured;

public sealed class SetPropertyFeaturedCommandHandler : IRequestHandler<SetPropertyFeaturedCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public SetPropertyFeaturedCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(SetPropertyFeaturedCommand request, CancellationToken cancellationToken)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Property), request.Id);

        property.SetFeatured(request.IsFeatured);
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
