using FluentValidation;

namespace VietPropEstate.Application.Features.Properties.Commands.UpdateProperty;

public class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
{
    public UpdatePropertyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Property ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Area)
            .GreaterThan(0).WithMessage("Area must be greater than zero.");
    }
}
