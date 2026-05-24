using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

/// <summary>A user's saved / favorited property listing.</summary>
public class Favorite : AuditableEntity
{
    public string UserId { get; private set; } = string.Empty;
    public Guid PropertyId { get; private set; }
    public string? Note { get; private set; }

    public Property Property { get; private set; } = null!;

    private Favorite() { }

    public static Favorite Create(string userId, Guid propertyId, string? note = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("User ID is required to create a favorite.");

        if (propertyId == Guid.Empty)
            throw new DomainException("Property ID is required to create a favorite.");

        return new Favorite
        {
            UserId = userId,
            PropertyId = propertyId,
            Note = note
        };
    }

    public void UpdateNote(string? note) => Note = note;
}
