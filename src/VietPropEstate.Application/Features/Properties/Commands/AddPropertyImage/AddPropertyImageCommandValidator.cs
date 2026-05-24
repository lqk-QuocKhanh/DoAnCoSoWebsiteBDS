using FluentValidation;

namespace VietPropEstate.Application.Features.Properties.Commands.AddPropertyImage;

public class AddPropertyImageCommandValidator : AbstractValidator<AddPropertyImageCommand>
{
    public AddPropertyImageCommandValidator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Image URL is required.")
            .MaximumLength(2000)
            .Must(u =>
                Uri.TryCreate(u, UriKind.Absolute, out _) ||
                u.StartsWith("/", StringComparison.Ordinal))
            .WithMessage("Image URL must be a valid absolute or root-relative path.");
        RuleFor(x => x.Caption).MaximumLength(500).When(x => x.Caption is not null);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
