namespace VietPropEstate.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(
        string toEmail,
        string userName,
        string userId,
        string token,
        CancellationToken cancellationToken = default);

    Task SendPasswordResetAsync(
        string toEmail,
        string userName,
        string resetLink,
        CancellationToken cancellationToken = default);

    Task SendWelcomeEmailAsync(
        string toEmail,
        string userName,
        CancellationToken cancellationToken = default);

    Task SendGenericAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
