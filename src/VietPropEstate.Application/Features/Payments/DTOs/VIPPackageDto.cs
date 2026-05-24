namespace VietPropEstate.Application.Features.Payments.DTOs;

public sealed class VIPPackageDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "VND";
    public int DurationDays { get; init; }
    public int MaxListings { get; init; }
    public bool IsActive { get; init; }
}
