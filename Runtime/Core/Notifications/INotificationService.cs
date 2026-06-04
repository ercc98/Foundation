using System;

namespace ErccDev.Foundation.Core.Notifications
{
    /// <summary>
    /// Push/notify surface for in-game toasts. Game code and sources push through it; views
    /// subscribe to <see cref="OnNotification"/> to render. The base manager implements it;
    /// swap in a fake for tests.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>Queue a notification to be shown.</summary>
        void Notify(in NotificationData data);

        /// <summary>Raised each time a queued notification becomes the active one.</summary>
        event Action<NotificationData> OnNotification;
    }
}
