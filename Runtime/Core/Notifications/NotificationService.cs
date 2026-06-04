using System;

namespace ErccDev.Foundation.Core.Notifications
{
    /// <summary>
    /// Static facade over an INotificationService. Lets any game push a toast in one line
    /// (e.g. a custom reward popup) without holding a reference to the manager. A null-object
    /// default keeps calls safe before a manager exists; the scene manager registers itself
    /// as the default in OnEnable.
    /// </summary>
    public static class NotificationService
    {
        private static INotificationService _default = new NullNotificationService();

        /// <summary>Current default notification service (a safe null-object until a manager registers).</summary>
        public static INotificationService Default => _default;

        /// <summary>
        /// Allows swapping the implementation (the scene manager, a fake for tests, etc).
        /// </summary>
        public static void SetDefault(INotificationService customService)
        {
            if (customService != null)
                _default = customService;
        }

        // ---------- Convenience API ----------

        public static void Notify(in NotificationData data)
            => _default.Notify(data);

        public static void Notify(
            string               title,
            string               message  = null,
            UnityEngine.Sprite   icon     = null,
            NotificationCategory category = NotificationCategory.Info,
            float                duration = NotificationData.DefaultDuration)
            => _default.Notify(new NotificationData(title, message, icon, category, duration));

        // ---------- Null object ----------

        /// <summary>Safe default: re-broadcasts so listeners still work even with no manager wired.</summary>
        private sealed class NullNotificationService : INotificationService
        {
            public event Action<NotificationData> OnNotification;
            public void Notify(in NotificationData data) => OnNotification?.Invoke(data);
        }
    }
}
