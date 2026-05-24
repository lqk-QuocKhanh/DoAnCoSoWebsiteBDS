using MediatR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.VerifyPhoneNumber;

public sealed class VerifyPhoneNumberCommandHandler
    : IRequestHandler<VerifyPhoneNumberCommand, UserProfileDto>
{
    private readonly IAuthService _authService;

    public VerifyPhoneNumberCommandHandler(IAuthService authService)
        => _authService = authService;

    public Task<UserProfileDto> Handle(
        VerifyPhoneNumberCommand request,
        CancellationToken cancellationToken)
        => _authService.VerifyPhoneNumberAsync(
            request.UserId,
            request.PhoneNumber,
            request.Code,
            cancellationToken);
}
