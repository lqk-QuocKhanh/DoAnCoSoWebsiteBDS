using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

public class PropertyImage : AuditableEntity
{
    public Guid PropertyId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string? Caption { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsPrimary { get; private set; }

    public Property Property { get; private set; } = null!;

    private PropertyImage() { }

    public static PropertyImage Create(Guid propertyId, string url, string? caption = null, int displayOrder = 0, bool isPrimary = false)
    {
        if (propertyId == Guid.Empty)
            throw new DomainException("Property ID is required for an image.");
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("Image URL is required.");

        return new PropertyImage
        {
            PropertyId = propertyId,
            Url = url,
            Caption = caption,
            DisplayOrder = displayOrder,
            IsPrimary = isPrimary
        };
    }

    public void SetAsPrimary() => IsPrimary = true;
    public void ClearPrimary() => IsPrimary = false;
    public void UpdateOrder(int order) => DisplayOrder = order;
}
