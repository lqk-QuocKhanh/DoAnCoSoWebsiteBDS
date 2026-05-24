using FluentValidation;

namespace VietPropEstate.Application.Features.Chat.Commands.SendMessage;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.SenderId).NotEmpty();
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content cannot be empty.")
            .MaximumLength(4000);
        RuleFor(x => x.AttachmentUrl)
            .MaximumLength(2000)
            .Must(u => u == null || Uri.TryCreate(u, UriKind.Absolute, out _))
            .When(x => x.AttachmentUrl is not null)
            .WithMessage("Attachment URL must be a valid absolute URL.");
    }
}
