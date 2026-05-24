using FluentValidation;

namespace VietPropEstate.Application.Features.Chat.Commands.StartConversation;

public sealed class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
{
    public StartConversationCommandValidator()
    {
        RuleFor(x => x.PropertyId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.SellerId).NotEmpty();
        RuleFor(x => x.Subject).MaximumLength(500).When(x => x.Subject is not null);
        RuleFor(x => x.InitialMessage).MaximumLength(4000).When(x => x.InitialMessage is not null);
        RuleFor(x => x).Must(x => x.BuyerId != x.SellerId)
            .WithMessage("Buyer and seller cannot be the same user.");
    }
}
