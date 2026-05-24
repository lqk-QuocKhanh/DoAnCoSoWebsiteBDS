using VietPropEstate.Domain.Common;

namespace VietPropEstate.Domain.Entities;

public class PropertyType : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Property> _properties = [];
    public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();

    private PropertyType() { }

    public static PropertyType Create(string name, string? description = null)
    {
        return new PropertyType
        {
            Name = name,
            Description = description,
            IsActive = true
        };
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
