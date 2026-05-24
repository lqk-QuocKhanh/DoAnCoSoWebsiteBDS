using FluentValidation;
using VietPropEstate.Application.Common.Helpers;

namespace VietPropEstate.Application.Features.Auth.Commands.VerifyPhoneNumber;

public sealed class VerifyPhoneNumberCommandValidator
    : AbstractValidator<VerifyPhoneNumberCommand>
{
    public VerifyPhoneNumberCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Số điện thoại là bắt buộc.")
            .Must(PhoneNumberHelper.IsValidVietnameseMobile)
            .WithMessage("Số điện thoại không hợp lệ.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Mã xác thực là bắt buộc.")
            .Length(6).WithMessage("Mã xác thực phải gồm 6 chữ số.");
    }
}
