namespace VietPropEstate.Application.Features.Addresses.DTOs;

public sealed class ProvinceLookupDto
{
    public int Code { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Codename { get; init; } = string.Empty;
    public string DivisionType { get; init; } = string.Empty;
}

public sealed class WardLookupDto
{
    public int Code { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Codename { get; init; } = string.Empty;
    public string DivisionType { get; init; } = string.Empty;
    public int ProvinceCode { get; init; }
}
