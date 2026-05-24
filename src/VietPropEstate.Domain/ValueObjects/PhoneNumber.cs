using System.Text.RegularExpressions;
using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Exceptions;

namespace VietPropEstate.Domain.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex VietnamPhoneRegex =
        new(@"^(\+84|84|0)(3[2-9]|5[6-9]|7[06-9]|8[0-689]|9[0-9])\d{7}$",
            RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    public string Value { get; }

    private PhoneNumber() { Value = string.Empty; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Phone number is required.");

        var normalized = value.Trim().Replace(" ", "").Replace("-", "");

        if (!VietnamPhoneRegex.IsMatch(normalized))
            throw new DomainException($"'{value}' is not a valid Vietnamese phone number.");

        Value = normalized;
    }

    public static PhoneNumber From(string value) => new(value);

    public string Formatted => Value.StartsWith('+') ? Value : Value.StartsWith("84") ? "+" + Value : Value;

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
