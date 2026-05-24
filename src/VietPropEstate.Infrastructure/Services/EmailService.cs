using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using VietPropEstate.Application.Common.Interfaces;

namespace VietPropEstate.Infrastructure.Services;

/// <summary>
/// SMTP email service with structured HTML templates.
/// Configure via EmailSettings in appsettings.json.
/// Set SmtpHost to empty to run in log-only (development) mode.
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailConfirmationAsync(
        string toEmail, string userName, string userId, string token,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = _configuration["EmailSettings:AppBaseUrl"] ?? "https://localhost:5001";
        var encodedToken = Uri.EscapeDataString(token);
        var confirmationLink = $"{baseUrl}/verify-email?userId={userId}&token={encodedToken}";

        var body = EmailTemplate(
            "Confirm your email address",
            $"Hello {EscapeHtml(userName)},",
            "Thank you for registering with VietPropEstate. Please confirm your email address by clicking the button below.",
            confirmationLink,
            "Confirm Email");

        await SendAsync(toEmail, "Confirm your VietPropEstate email address", body, cancellationToken);
    }

    public async Task SendPasswordResetAsync(
        string toEmail, string userName, string resetLink,
        CancellationToken cancellationToken = default)
    {
        var body = EmailTemplate(
            "Reset your password",
            $"Hello {EscapeHtml(userName)},",
            "We received a request to reset your password. Click the button below to set a new password. This link expires in 1 hour.",
            resetLink,
            "Reset Password");

        await SendAsync(toEmail, "VietPropEstate — Reset your password", body, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(
        string toEmail, string userName,
        CancellationToken cancellationToken = default)
    {
        var body = $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px'>
          <h2 style='color:#1a56db'>Welcome to VietPropEstate 🎉</h2>
          <p>Hello {EscapeHtml(userName)},</p>
          <p>Your account has been created successfully. You can now browse thousands of
             property listings across Vietnam.</p>
          <p>If you have any questions, please contact us at support@vietpropestate.vn.</p>
          <hr style='border:none;border-top:1px solid #e5e7eb;margin:24px 0'/>
          <p style='color:#6b7280;font-size:12px'>
            VietPropEstate · Vietnam Property Platform
          </p>
        </div>";

        await SendAsync(toEmail, "Welcome to VietPropEstate!", body, cancellationToken);
    }

    public async Task SendGenericAsync(
        string toEmail, string subject, string htmlBody,
        CancellationToken cancellationToken = default)
        => await SendAsync(toEmail, subject, htmlBody, cancellationToken);

    // ─── Private ─────────────────────────────────────────────────────────────

    private async Task SendAsync(
        string toEmail, string subject, string htmlBody,
        CancellationToken cancellationToken)
    {
        var settings = GetEmailSettings();

        if (string.IsNullOrWhiteSpace(settings.SmtpHost))
        {
            // Development mode: log the email instead of sending
            _logger.LogInformation(
                "[EMAIL LOG] To: {To} | Subject: {Subject}\n{Body}",
                toEmail, subject, htmlBody);
            return;
        }

        try
        {
            using var client = new SmtpClient(settings.SmtpHost, settings.SmtpPort)
            {
                EnableSsl = settings.EnableSsl,
                Credentials = new NetworkCredential(settings.Username, settings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using var message = new MailMessage
            {
                From = new MailAddress(settings.SenderEmail, settings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email sent to {To}: {Subject}.", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}: {Subject}.", toEmail, subject);
            throw;
        }
    }

    private static string EmailTemplate(
        string heading, string greeting, string body, string actionUrl, string actionText)
    {
        return $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;padding:20px'>
          <h2 style='color:#1a56db'>{EscapeHtml(heading)}</h2>
          <p>{EscapeHtml(greeting)}</p>
          <p>{EscapeHtml(body)}</p>
          <div style='text-align:center;margin:32px 0'>
            <a href='{actionUrl}'
               style='background:#1a56db;color:#fff;padding:14px 28px;
                      border-radius:6px;text-decoration:none;font-weight:600'>
              {EscapeHtml(actionText)}
            </a>
          </div>
          <p style='color:#6b7280;font-size:13px'>
            If you did not request this, please ignore this email.
          </p>
          <hr style='border:none;border-top:1px solid #e5e7eb;margin:24px 0'/>
          <p style='color:#6b7280;font-size:12px'>VietPropEstate · Vietnam Property Platform</p>
        </div>";
    }

    private static string EscapeHtml(string text) =>
        text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    private EmailSettings GetEmailSettings() => new()
    {
        SmtpHost = _configuration["EmailSettings:SmtpHost"] ?? string.Empty,
        SmtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587"),
        SenderEmail = _configuration["EmailSettings:SenderEmail"] ?? "noreply@vietpropestate.vn",
        SenderName = _configuration["EmailSettings:SenderName"] ?? "VietPropEstate",
        Username = _configuration["EmailSettings:Username"] ?? string.Empty,
        Password = _configuration["EmailSettings:Password"] ?? string.Empty,
        EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true"),
        AppBaseUrl = _configuration["EmailSettings:AppBaseUrl"] ?? "https://localhost:5001"
    };

    private sealed record EmailSettings
    {
        public string SmtpHost { get; init; } = string.Empty;
        public int SmtpPort { get; init; } = 587;
        public string SenderEmail { get; init; } = string.Empty;
        public string SenderName { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public bool EnableSsl { get; init; } = true;
        public string AppBaseUrl { get; init; } = string.Empty;
    }
}
