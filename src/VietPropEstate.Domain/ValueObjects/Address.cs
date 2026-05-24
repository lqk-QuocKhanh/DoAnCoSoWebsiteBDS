using VietPropEstate.Domain.Common;

namespace VietPropEstate.Domain.ValueObjects;

public sealed class Address : ValueObject
{
    public string Street { get; }
    public string Ward { get; }
    public string District { get; }
    public string Province { get; }
    public string Country { get; }
    public double? Latitude { get; }
    public double? Longitude { get; }

    private Address()
    {
        Street = string.Empty;
        Ward = string.Empty;
        District = string.Empty;
        Province = string.Empty;
        Country = string.Empty;
    }

    public Address(
        string street,
        string ward,
        string district,
        string province,
        string country = "Vietnam",
        double? latitude = null,
        double? longitude = null)
    {
        Street = street;
        Ward = ward;
        District = district;
        Province = province;
        Country = country;
        Latitude = latitude;
        Longitude = longitude;
    }

    public string FullAddress => $"{Street}, {Ward}, {District}, {Province}, {Country}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return Ward;
        yield return District;
        yield return Province;
        yield return Country;
    }
}
