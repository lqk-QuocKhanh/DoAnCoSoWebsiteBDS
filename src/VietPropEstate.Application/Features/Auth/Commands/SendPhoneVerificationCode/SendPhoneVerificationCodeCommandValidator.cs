using FluentValidation;
using VietPropEstate.Application.Common.Helpers;

namespace VietPropEstate.Application.Features.Auth.Commands.SendPhoneVerificationCode;

public sealed class SendPhoneVerificationCodeCommandValidator
    : AbstractValidator<SendPhoneVerificationCodeCommand>
{
    public SendPhoneVerificationCodeCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Số điện thoại là bắt buộc.")
            .Must(PhoneNumberHelper.IsValidVietnameseMobile)
            .WithMessage("Số điện thoại không hợp lệ. Vui lòng nhập số di động Việt Nam (10 chữ số).");
    }
}
