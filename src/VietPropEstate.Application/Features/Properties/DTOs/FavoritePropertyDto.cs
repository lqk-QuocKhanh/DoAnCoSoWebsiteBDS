namespace VietPropEstate.Application.Features.Properties.DTOs;

public class FavoritePropertyDto
{
    public Guid FavoriteId { get; set; }
    public string? Note { get; set; }
    public DateTime FavoritedAt { get; set; }
    public PropertyDto Property { get; set; } = null!;
}
