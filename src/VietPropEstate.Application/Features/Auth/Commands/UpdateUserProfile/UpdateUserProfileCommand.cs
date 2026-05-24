using MediatR;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand : IRequest<UserProfileDto>
{
    public string UserId { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? AddressLine { get; init; }
    public string? ProvinceName { get; init; }
    public string? WardName { get; init; }
    public int? ProvinceCode { get; init; }
    public int? WardCode { get; init; }
}
