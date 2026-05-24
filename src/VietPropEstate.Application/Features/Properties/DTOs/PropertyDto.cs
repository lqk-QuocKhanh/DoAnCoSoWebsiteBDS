using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Properties.DTOs;

public class PropertyDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal PriceAmount { get; set; }
    public string PriceCurrency { get; set; } = "VND";
    public decimal Area { get; set; }
    public int? NumberOfBedrooms { get; set; }
    public int? NumberOfBathrooms { get; set; }
    public PropertyStatus Status { get; set; }
    public ListingType ListingType { get; set; }
    public string? FullAddress { get; set; }
    public string? ProvinceName { get; set; }
    public int? ProvinceCode { get; set; }
    public string? WardName { get; set; }
    public int? WardCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public Guid PropertyTypeId { get; set; }
    public string? PropertyTypeName { get; set; }
    public Guid? TransactionTypeId { get; set; }
    public string? TransactionTypeName { get; set; }
    public Guid AgentId { get; set; }
    public string? AgentName { get; set; }

    // Backward-compatibility shims for Blazor UI
    public string Province => ProvinceName ?? string.Empty;
    public string District => WardName ?? string.Empty;
}
