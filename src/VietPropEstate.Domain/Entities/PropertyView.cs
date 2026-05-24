using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

/// <summary>Records a single view of a property listing (supports both authenticated and anonymous users).</summary>
public class PropertyView : BaseEntity
{
    public Guid PropertyId { get; private set; }

    /// <summary>Identity user ID — null for anonymous visitors.</summary>
    public string? UserId { get; private set; }

    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? SessionId { get; private set; }
    public DateTime ViewedAt { get; private set; }

    public Property Property { get; private set; } = null!;

    private PropertyView() { }

    public static PropertyView Record(
        Guid propertyId,
        string? userId = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? sessionId = null)
    {
        if (propertyId == Guid.Empty)
            throw new DomainException("Property ID is required to record a view.");

        return new PropertyView
        {
            PropertyId = propertyId,
            UserId = userId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            SessionId = sessionId,
            ViewedAt = DateTime.UtcNow
        };
    }
}
