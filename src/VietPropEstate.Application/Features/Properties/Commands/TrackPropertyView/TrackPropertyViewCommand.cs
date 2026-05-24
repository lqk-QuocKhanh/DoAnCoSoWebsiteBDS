using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.TrackPropertyView;

public record TrackPropertyViewCommand : IRequest<Unit>
{
    public Guid PropertyId { get; init; }
    public string? UserId { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public string? SessionId { get; init; }
}
