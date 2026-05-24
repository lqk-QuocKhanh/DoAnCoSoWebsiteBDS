using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.AddPropertyImage;

public record AddPropertyImageCommand : IRequest<Guid>
{
    public Guid PropertyId { get; init; }
    public string Url { get; init; } = string.Empty;
    public string? Caption { get; init; }
    public int DisplayOrder { get; init; } = 0;
    public bool IsPrimary { get; init; } = false;
}
