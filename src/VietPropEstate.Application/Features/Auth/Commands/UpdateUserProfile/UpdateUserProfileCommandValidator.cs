using FluentValidation;

namespace VietPropEstate.Application.Features.Auth.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(100).When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .MaximumLength(100).When(x => x.LastName is not null);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).When(x => x.PhoneNumber is not null)
            .Matches(@"^[\d\s+\-()]*$").When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Số điện thoại không hợp lệ.");

        RuleFor(x => x.AddressLine)
            .MaximumLength(500).When(x => x.AddressLine is not null);

        RuleFor(x => x.ProvinceName)
            .MaximumLength(200).When(x => x.ProvinceName is not null);

        RuleFor(x => x.WardName)
            .MaximumLength(200).When(x => x.WardName is not null);
    }
}
