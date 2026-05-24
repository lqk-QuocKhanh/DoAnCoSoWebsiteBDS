using VietPropEstate.Domain.Common;

namespace VietPropEstate.Domain.Events;

public sealed class PropertyCreatedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid PropertyId { get; }

    public PropertyCreatedEvent(Guid propertyId)
    {
        PropertyId = propertyId;
    }
}
