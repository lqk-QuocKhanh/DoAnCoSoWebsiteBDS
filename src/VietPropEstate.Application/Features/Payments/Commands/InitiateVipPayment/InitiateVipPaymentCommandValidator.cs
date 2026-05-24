using FluentValidation;

namespace VietPropEstate.Application.Features.Payments.Commands.InitiateVipPayment;

public sealed class InitiateVipPaymentCommandValidator : AbstractValidator<InitiateVipPaymentCommand>
{
    public InitiateVipPaymentCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.VIPPackageId).NotEmpty();
        RuleFor(x => x.Locale)
            .Must(l => l is "vn" or "en")
            .WithMessage("Locale must be 'vn' or 'en'.");
    }
}
