namespace ErccDev.Foundation.Core.Notifications
{
    /// <summary>
    /// Coarse "kind" of a notification so views can pick an icon tint, sound or layout
    /// via a switch — without the notification core knowing about any concrete game system.
    /// </summary>
    public enum NotificationCategory
    {
        Info,
        Achievement,
        Reward,
        Collection,
        Custom
    }
}
