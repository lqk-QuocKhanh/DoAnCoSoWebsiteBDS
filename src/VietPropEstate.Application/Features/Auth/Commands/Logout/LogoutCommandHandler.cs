using MediatR;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        => _authService.LogoutAsync(request.UserId, request.IpAddress, cancellationToken);
}
