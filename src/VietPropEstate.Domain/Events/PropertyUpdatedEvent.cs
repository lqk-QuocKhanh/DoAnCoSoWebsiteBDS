using VietPropEstate.Domain.Common;

namespace VietPropEstate.Domain.Events;

public sealed class PropertyUpdatedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid PropertyId { get; }

    public PropertyUpdatedEvent(Guid propertyId)
    {
        PropertyId = propertyId;
    }
}
