using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.UpdatePropertyAddress;

public record UpdatePropertyAddressCommand : IRequest<Unit>
{
    public Guid PropertyId { get; init; }
    public int? ProvinceCode { get; init; }
    public string? ProvinceName { get; init; }
    public int? WardCode { get; init; }
    public string? WardName { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
}
