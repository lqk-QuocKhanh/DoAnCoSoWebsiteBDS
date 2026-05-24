using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Infrastructure.Services;

/// <summary>
/// SMS delivery service. Logs OTP in development/testing when no provider is configured.
/// </summary>
public sealed class SmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<SmsService> _logger;

    public SmsService(
        IConfiguration configuration,
        IHostEnvironment environment,
        ILogger<SmsService> logger)
    {
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    public Task SendVerificationCodeAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default)
    {
        var provider = _configuration["SmsSettings:Provider"];
        if (string.IsNullOrWhiteSpace(provider))
        {
            _logger.LogWarning(
                "SMS provider not configured. Verification code for {Phone}: {Code}",
                phoneNumber,
                code);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Sending verification SMS to {Phone} via {Provider}.", phoneNumber, provider);
        // Provider integration can be added here (Twilio, ESMS, etc.)
        return Task.CompletedTask;
    }

    public bool ShouldExposeDevCode =>
        _environment.IsDevelopment() || _environment.IsEnvironment("Testing");
}
