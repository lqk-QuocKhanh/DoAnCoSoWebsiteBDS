using FluentValidation;

namespace VietPropEstate.Application.Features.Properties.Commands.CreateProperty;

public class CreatePropertyCommandValidator : AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(500);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Area)
            .GreaterThan(0).WithMessage("Area must be greater than zero.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street address is required.")
            .MaximumLength(500)
            .When(x => !x.ProvinceCode.HasValue);

        RuleFor(x => x.Ward)
            .NotEmpty().WithMessage("Ward is required.")
            .MaximumLength(200)
            .When(x => !x.WardCode.HasValue);

        RuleFor(x => x.District)
            .MaximumLength(200);

        RuleFor(x => x.Province)
            .NotEmpty().WithMessage("Province is required.")
            .MaximumLength(200)
            .When(x => !x.ProvinceCode.HasValue);

        RuleFor(x => x.PropertyTypeId)
            .NotEmpty().WithMessage("Property type is required.");

        RuleFor(x => x.ProvinceCode)
            .GreaterThan(0).When(x => x.ProvinceCode.HasValue);

        RuleFor(x => x.WardCode)
            .GreaterThan(0).When(x => x.WardCode.HasValue);

        RuleFor(x => x.NumberOfBedrooms)
            .GreaterThanOrEqualTo(0).When(x => x.NumberOfBedrooms.HasValue);

        RuleFor(x => x.NumberOfBathrooms)
            .GreaterThanOrEqualTo(0).When(x => x.NumberOfBathrooms.HasValue);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
    }
}
