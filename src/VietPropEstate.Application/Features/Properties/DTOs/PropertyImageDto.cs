namespace VietPropEstate.Application.Features.Properties.DTOs;

public class PropertyImageDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}
