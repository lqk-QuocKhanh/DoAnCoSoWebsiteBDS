namespace VietPropEstate.Application.Features.Payments.DTOs;

public sealed class UserVIPPackageDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public Guid VIPPackageId { get; init; }
    public string? VIPPackageName { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int RemainingListings { get; init; }
    public bool IsActive { get; init; }
    public bool IsExpired { get; init; }
    public int DaysRemaining { get; init; }
}
