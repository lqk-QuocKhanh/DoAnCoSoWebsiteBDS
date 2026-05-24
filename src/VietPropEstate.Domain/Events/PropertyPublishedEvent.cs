using VietPropEstate.Domain.Common;

namespace VietPropEstate.Domain.Events;

public sealed class PropertyPublishedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid PropertyId { get; }

    public PropertyPublishedEvent(Guid propertyId)
    {
        PropertyId = propertyId;
    }
}
