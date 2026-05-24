using FluentValidation;

using VietPropEstate.Application.Common.Authorization;

namespace VietPropEstate.Application.Features.Auth.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private static readonly string[] AllowedPublicRoles = AppRoles.SelfRegisterable;

    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(100)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.FirstName)
            .MaximumLength(100).When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .MaximumLength(100).When(x => x.LastName is not null);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).When(x => x.PhoneNumber is not null);

        RuleFor(x => x.Role)
            .Must(role => AllowedPublicRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Role must be one of: {string.Join(", ", AllowedPublicRoles)}.");
    }
}
