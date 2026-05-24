using MediatR;
using VietPropEstate.Application.Features.Chat.DTOs;

namespace VietPropEstate.Application.Features.Chat.Commands.UpdateConversationSettings;

public sealed class UpdateConversationSettingsCommand : IRequest<ConversationDto>
{
    public Guid ConversationId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public bool? IsPinned { get; init; }
    public bool? IsMuted { get; init; }
}
