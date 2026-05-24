using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

/// <summary>Lookup entity for property transaction types (Sell / Rent).</summary>
public class TransactionType : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Property> _properties = [];
    public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();

    private TransactionType() { }

    public static TransactionType Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Transaction type name is required.");

        return new TransactionType
        {
            Name = name.Trim(),
            Description = description,
            IsActive = true
        };
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Transaction type name is required.");

        Name = name.Trim();
        Description = description;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
