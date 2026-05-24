using MediatR;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.TrackPropertyView;

public sealed class TrackPropertyViewCommandHandler : IRequestHandler<TrackPropertyViewCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public TrackPropertyViewCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(TrackPropertyViewCommand request, CancellationToken cancellationToken)
    {
        var view = PropertyView.Record(
            request.PropertyId, request.UserId,
            request.IpAddress, request.UserAgent, request.SessionId);

        await _unitOfWork.PropertyViews.AddAsync(view, cancellationToken);
        await _unitOfWork.Properties.IncrementViewCountAsync(request.PropertyId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
