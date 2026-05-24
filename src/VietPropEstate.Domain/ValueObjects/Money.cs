using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money() { Currency = string.Empty; }

    public Money(decimal amount, string currency = "VND")
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "VND") => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot add {Currency} and {other.Currency}.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot subtract {other.Currency} from {Currency}.");
        return new Money(Amount - other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:N0} {Currency}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
