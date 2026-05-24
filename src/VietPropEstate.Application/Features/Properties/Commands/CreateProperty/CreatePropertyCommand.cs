using MediatR;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Properties.Commands.CreateProperty;

public record CreatePropertyCommand : IRequest<Guid>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "VND";
    public decimal Area { get; init; }
    public ListingType ListingType { get; init; }
    public int? NumberOfBedrooms { get; init; }
    public int? NumberOfBathrooms { get; init; }
    public int? NumberOfFloors { get; init; }
    public PropertyDirection? Direction { get; init; }

    // Legacy free-text address
    public string Street { get; init; } = string.Empty;
    public string Ward { get; init; } = string.Empty;
    public string District { get; init; } = string.Empty;
    public string Province { get; init; } = string.Empty;

    // Vietnam structured address
    public int? ProvinceCode { get; init; }
    public string? ProvinceName { get; init; }
    public int? WardCode { get; init; }
    public string? WardName { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }

    public Guid PropertyTypeId { get; init; }
    public Guid? TransactionTypeId { get; init; }
    public Guid AgentId { get; init; }
    public string? VideoUrl { get; init; }
}
