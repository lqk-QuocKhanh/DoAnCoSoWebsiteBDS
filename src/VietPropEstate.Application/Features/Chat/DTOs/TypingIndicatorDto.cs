namespace VietPropEstate.Application.Features.Chat.DTOs;

public sealed record TypingIndicatorDto(
    Guid ConversationId,
    string UserId,
    string? UserName,
    bool IsTyping);
