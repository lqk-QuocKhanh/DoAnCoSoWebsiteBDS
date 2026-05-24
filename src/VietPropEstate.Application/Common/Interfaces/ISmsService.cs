namespace VietPropEstate.Application.Common.Interfaces;

public interface ISmsService
{
    Task SendVerificationCodeAsync(
        string phoneNumber,
        string code,
        CancellationToken cancellationToken = default);
}
