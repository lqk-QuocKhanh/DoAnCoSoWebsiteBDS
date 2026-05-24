using MediatR;
using VietPropEstate.Application.Common.Interfaces;
using VietPropEstate.Application.Features.Auth.DTOs;

namespace VietPropEstate.Application.Features.Auth.Commands.SendPhoneVerificationCode;

public sealed class SendPhoneVerificationCodeCommandHandler
    : IRequestHandler<SendPhoneVerificationCodeCommand, SendPhoneVerificationResponseDto>
{
    private readonly IAuthService _authService;

    public SendPhoneVerificationCodeCommandHandler(IAuthService authService)
        => _authService = authService;

    public Task<SendPhoneVerificationResponseDto> Handle(
        SendPhoneVerificationCodeCommand request,
        CancellationToken cancellationToken)
        => _authService.SendPhoneVerificationCodeAsync(
            request.UserId,
            request.PhoneNumber,
            cancellationToken);
}
