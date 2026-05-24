using VietPropEstate.Application.Common.Models;
using VietPropEstate.Application.Features.Properties.DTOs;

namespace VietPropEstate.Application.Features.Properties.DTOs;

public sealed class PosterProfileDto
{
    public string UserId { get; init; } = string.Empty;
    public Guid AgentId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? AvatarUrl { get; set; }
    public string? AgencyName { get; init; }
    public string? Bio { get; init; }
    public DateTime MemberSince { get; init; }
    public int TotalListings { get; init; }
    public PaginatedList<PropertyDto> Listings { get; set; } = null!;
}
