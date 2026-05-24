using FluentValidation;

namespace VietPropEstate.Application.Features.Properties.Queries.GetPropertiesList;

public class GetPropertiesListQueryValidator : AbstractValidator<GetPropertiesListQuery>
{
    private static readonly string[] ValidSortFields = ["createdat", "price", "area", "views", "published"];
    private static readonly string[] ValidSortOrders = ["asc", "desc"];

    public GetPropertiesListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);
        RuleFor(x => x.MaxPrice)
            .GreaterThan(x => x.MinPrice ?? 0)
            .When(x => x.MaxPrice.HasValue);
        RuleFor(x => x.MinArea).GreaterThan(0).When(x => x.MinArea.HasValue);
        RuleFor(x => x.MaxArea)
            .GreaterThan(x => x.MinArea ?? 0)
            .When(x => x.MaxArea.HasValue);
        RuleFor(x => x.MinBedrooms).GreaterThanOrEqualTo(0).When(x => x.MinBedrooms.HasValue);
        RuleFor(x => x.MaxBedrooms)
            .GreaterThanOrEqualTo(x => x.MinBedrooms ?? 0)
            .When(x => x.MaxBedrooms.HasValue);
        RuleFor(x => x.SortBy)
            .Must(s => ValidSortFields.Contains(s.ToLower()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", ValidSortFields)}.");
        RuleFor(x => x.SortOrder)
            .Must(s => ValidSortOrders.Contains(s.ToLower()))
            .WithMessage("SortOrder must be 'Asc' or 'Desc'.");
    }
}
