using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Domain.Events;

public sealed class PropertyStatusChangedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid PropertyId { get; }
    public PropertyStatus NewStatus { get; }

    public PropertyStatusChangedEvent(Guid propertyId, PropertyStatus newStatus)
    {
        PropertyId = propertyId;
        NewStatus = newStatus;
    }
}
