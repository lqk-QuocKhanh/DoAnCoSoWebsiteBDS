namespace VietPropEstate.Application.Features.Agents.DTOs;

public class AgentDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AgencyName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
}

public class AgentDetailDto : AgentDto
{
    public string? LicenseNumber { get; set; }
    public string? Bio { get; set; }
    public int PropertyCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
