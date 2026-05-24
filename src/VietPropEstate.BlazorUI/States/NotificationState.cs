namespace VietPropEstate.BlazorUI.States;

public sealed class NotificationState
{
    public int UnreadMessages { get; private set; }
    public int UnreadNotifications { get; private set; }
    public int TotalUnread => UnreadMessages + UnreadNotifications;

    public event Action? OnChange;

    public void SetCounts(int messages, int notifications)
    {
        UnreadMessages = messages;
        UnreadNotifications = notifications;
        OnChange?.Invoke();
    }

    public void DecrementMessage()
    {
        if (UnreadMessages > 0) UnreadMessages--;
        OnChange?.Invoke();
    }

    public void DecrementNotification()
    {
        if (UnreadNotifications > 0) UnreadNotifications--;
        OnChange?.Invoke();
    }
}
