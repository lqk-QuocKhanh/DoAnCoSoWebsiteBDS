using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Application.Common.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int SlowRequestThresholdMs = 500;

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            _logger.LogWarning(
                "VietPropEstate slow request: {RequestName} ({ElapsedMilliseconds}ms) UserId={UserId} Request={@Request}",
                typeof(TRequest).Name,
                stopwatch.ElapsedMilliseconds,
                _currentUserService.UserId,
                request);
        }

        return response;
    }
}
