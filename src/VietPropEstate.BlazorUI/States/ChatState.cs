using VietPropEstate.Application.Features.Chat.DTOs;

namespace VietPropEstate.BlazorUI.States;

public sealed class ChatState
{
    public List<ConversationDto> Conversations { get; private set; } = [];
    public ConversationDto? ActiveConversation { get; private set; }
    public List<MessageDto> ActiveMessages { get; private set; } = [];
    public bool IsTyping { get; private set; }
    public string? TypingUserId { get; private set; }

    public event Action? OnChange;

    public void SetConversations(List<ConversationDto> conversations)
    {
        Conversations = conversations;
        OnChange?.Invoke();
    }

    public void SetActiveConversation(ConversationDto? conversation)
    {
        ActiveConversation = conversation;
        ActiveMessages = [];
        OnChange?.Invoke();
    }

    public void SetMessages(List<MessageDto> messages)
    {
        ActiveMessages = messages;
        OnChange?.Invoke();
    }

    public void AddMessage(MessageDto message)
    {
        ActiveMessages.Add(message);
        OnChange?.Invoke();
    }

    public void SetTyping(bool isTyping, string? userId = null)
    {
        IsTyping = isTyping;
        TypingUserId = userId;
        OnChange?.Invoke();
    }
}
