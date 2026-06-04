using UnityEngine;

namespace ErccDev.Foundation.Core.Notifications
{
    /// <summary>
    /// The single, generic payload the whole module passes around: just display data.
    /// Deliberately knows nothing about achievements/collections/rewards — sources translate
    /// their domain objects into this, views render it.
    /// </summary>
    public readonly struct NotificationData
    {
        public const float DefaultDuration = 2.5f;

        public readonly string               Title;
        public readonly string               Message;
        public readonly Sprite               Icon;
        public readonly NotificationCategory Category;
        public readonly float                Duration;   // seconds on screen; <= 0 chains immediately

        public NotificationData(
            string               title,
            string               message  = null,
            Sprite               icon     = null,
            NotificationCategory category = NotificationCategory.Info,
            float                duration = DefaultDuration)
        {
            Title    = title;
            Message  = message;
            Icon     = icon;
            Category = category;
            Duration = duration;
        }
    }
}
