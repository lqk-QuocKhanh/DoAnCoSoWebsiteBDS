using MediatR;

namespace VietPropEstate.Application.Features.Properties.Commands.ToggleFavorite;

public record ToggleFavoriteCommand : IRequest<bool>
{
    public string UserId { get; init; } = string.Empty;
    public Guid PropertyId { get; init; }
    public string? Note { get; init; }
}
