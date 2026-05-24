using MediatR;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.Application.Features.Properties.Commands.UpdateProperty;

public record UpdatePropertyCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "VND";
    public decimal Area { get; init; }
    public int? NumberOfBedrooms { get; init; }
    public int? NumberOfBathrooms { get; init; }
    public int? NumberOfFloors { get; init; }
    public PropertyDirection? Direction { get; init; }
    public string? VideoUrl { get; init; }
    public Guid? TransactionTypeId { get; init; }
}
