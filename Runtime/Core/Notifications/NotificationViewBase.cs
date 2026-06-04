using UnityEngine;

namespace ErccDev.Foundation.Core.Notifications
{
    /// <summary>
    /// Base for the actual on-screen toast. Subscribes to a notification service and forwards
    /// each notification to <see cref="Show"/>. The Foundation ships no visuals on purpose —
    /// subclass and bind your own prefab/animation. Wire the service in the inspector, or leave
    /// it empty to bind the <see cref="NotificationService"/> default.
    /// </summary>
    public abstract class NotificationViewBase : MonoBehaviour
    {
        [Header("Wiring")]
        [Tooltip("Optional: a NotificationManagerBase. Empty → uses the NotificationService default.")]
        [SerializeField] protected MonoBehaviour notificationService;   // INotificationService

        protected INotificationService _service;

        protected virtual void OnEnable()
        {
            _service = notificationService as INotificationService ?? NotificationService.Default;
            if (_service != null)
                _service.OnNotification += Show;
        }

        protected virtual void OnDisable()
        {
            if (_service != null)
                _service.OnNotification -= Show;
        }

        /// <summary>Render the notification. Called once per notification, in queue order.</summary>
        protected abstract void Show(NotificationData data);

        /// <summary>Hook for hiding/clearing the toast once its duration elapses.</summary>
        protected virtual void Hide() { }
    }
}
