using MediatR;
using VietPropEstate.Application.Common.Exceptions;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Interfaces;

namespace VietPropEstate.Application.Features.Properties.Commands.ToggleFavorite;

/// <summary>Adds the property to favorites if not already there; removes it if it is. Returns true = added, false = removed.</summary>
public sealed class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public ToggleFavoriteCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.Properties.ExistsAsync(
                p => p.Id == request.PropertyId, cancellationToken))
            throw new NotFoundException(nameof(Property), request.PropertyId);

        var existing = await _unitOfWork.Favorites.FirstOrDefaultAsync(
            f => f.UserId == request.UserId && f.PropertyId == request.PropertyId && !f.IsDeleted,
            cancellationToken);

        if (existing is not null)
        {
            // Remove
            _unitOfWork.Favorites.Remove(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return false;
        }

        // Add
        var favorite = Favorite.Create(request.UserId, request.PropertyId, request.Note);
        await _unitOfWork.Favorites.AddAsync(favorite, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
