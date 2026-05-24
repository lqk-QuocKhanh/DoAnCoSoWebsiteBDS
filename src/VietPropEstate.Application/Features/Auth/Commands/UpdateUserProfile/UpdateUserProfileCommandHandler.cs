using MediatR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandHandler
    : IRequestHandler<UpdateUserProfileCommand, UserProfileDto>
{
    private readonly IAuthService _authService;

    public UpdateUserProfileCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<UserProfileDto> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken)
        => _authService.UpdateProfileAsync(request, cancellationToken);
}
