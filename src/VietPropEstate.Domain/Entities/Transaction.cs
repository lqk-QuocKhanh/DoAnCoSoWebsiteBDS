using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.Exceptions;
using VietPropEstate.Domain.ValueObjects;

namespace VietPropEstate.Domain.Entities;

public class Transaction : AuditableEntity
{
    public Guid PropertyId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid AgentId { get; private set; }
    public ListingType TransactionType { get; private set; }
    public Money Amount { get; private set; } = Money.Zero();
    public Money? CommissionAmount { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }
    public string? ContractNumber { get; private set; }

    public Property Property { get; private set; } = null!;
    public Customer Customer { get; private set; } = null!;
    public Agent Agent { get; private set; } = null!;

    private Transaction() { }

    public static Transaction Create(
        Guid propertyId,
        Guid customerId,
        Guid agentId,
        ListingType transactionType,
        Money amount,
        string? contractNumber = null,
        string? notes = null)
    {
        return new Transaction
        {
            PropertyId = propertyId,
            CustomerId = customerId,
            AgentId = agentId,
            TransactionType = transactionType,
            Amount = amount,
            ContractNumber = contractNumber,
            Notes = notes,
            Status = TransactionStatus.Pending
        };
    }

    public void SetCommission(Money commissionAmount) => CommissionAmount = commissionAmount;

    public void Progress()
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Only pending transactions can be progressed.");

        Status = TransactionStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != TransactionStatus.InProgress)
            throw new DomainException("Only in-progress transactions can be completed.");

        Status = TransactionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == TransactionStatus.Completed)
            throw new DomainException("Completed transactions cannot be cancelled.");

        Status = TransactionStatus.Cancelled;
        Notes = $"{Notes}\nCancellation reason: {reason}".Trim();
    }

    public void UpdateNotes(string notes) => Notes = notes;
}
