using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.Entities;

public class Customer : AuditableEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public CustomerType CustomerType { get; private set; }
    public string? Notes { get; private set; }
    public string? UserId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Transaction> _transactions = [];
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    private Customer() { }

    public static Customer Create(
        string fullName,
        string email,
        string phoneNumber,
        CustomerType customerType,
        string? userId = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Customer full name is required.");

        return new Customer
        {
            FullName = fullName,
            Email = email,
            PhoneNumber = phoneNumber,
            CustomerType = customerType,
            UserId = userId,
            IsActive = true
        };
    }

    public void UpdateContact(string fullName, string email, string phoneNumber, string? notes)
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        Notes = notes;
    }

    public void ChangeType(CustomerType customerType) => CustomerType = customerType;

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
