using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Properties.DTOs;

public class PropertyDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PriceAmount { get; set; }
    public string PriceCurrency { get; set; } = "VND";
    public decimal Area { get; set; }
    public int? NumberOfBedrooms { get; set; }
    public int? NumberOfBathrooms { get; set; }
    public int? NumberOfFloors { get; set; }
    public PropertyStatus Status { get; set; }
    public ListingType ListingType { get; set; }
    public PropertyDirection? Direction { get; set; }

    // Legacy address (free-text)
    public string? Street { get; set; }
    public string? AddressWard { get; set; }
    public string? AddressDistrict { get; set; }
    public string? AddressProvince { get; set; }

    // Vietnam structured address
    public string? FullAddress { get; set; }
    public string? ProvinceName { get; set; }
    public int? ProvinceCode { get; set; }
    public string? WardName { get; set; }
    public int? WardCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? VideoUrl { get; set; }

    public Guid PropertyTypeId { get; set; }
    public string? PropertyTypeName { get; set; }
    public Guid? TransactionTypeId { get; set; }
    public string? TransactionTypeName { get; set; }
    public Guid AgentId { get; set; }
    public string? AgentUserId { get; set; }
    public string? AgentName { get; set; }
    public string? AgentPhone { get; set; }
    public string? AgentEmail { get; set; }
    public string? AgentAvatarUrl { get; set; }

    public List<PropertyImageDto> Images { get; set; } = [];
}
