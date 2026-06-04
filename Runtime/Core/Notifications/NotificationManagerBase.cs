using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ErccDev.Foundation.Core.Events;

namespace ErccDev.Foundation.Core.Notifications
{
    /// <summary>
    /// Generic notification engine: queues toasts and surfaces them one at a time, honoring
    /// each one's Duration. Knows nothing about what produced them — sources push data in,
    /// views subscribe to <see cref="OnNotification"/> and draw. Registers itself as the
    /// <see cref="NotificationService"/> default so game code can push from anywhere.
    /// Subclass to add custom queuing rules, analytics, sounds, etc.
    /// </summary>
    public class NotificationManagerBase : MonoBehaviour, INotificationService
    {
        [Header("Setup")]
        [Tooltip("Keep the manager alive across scene loads.")]
        [SerializeField] protected bool persistAcrossScenes = true;

        protected readonly Queue<NotificationData> _queue = new();
        protected bool _showing;

        public event Action<NotificationData> OnNotification;

        // ---------- Lifecycle ----------

        protected virtual void Awake()
        {
            if (persistAcrossScenes)
                DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnEnable()
        {
            NotificationService.SetDefault(this);
        }

        // ---------- INotificationService ----------

        public virtual void Notify(in NotificationData data)
        {
            _queue.Enqueue(data);
            if (!_showing) ShowNext();
        }

        // ---------- Queue pump ----------

        protected virtual void ShowNext()
        {
            if (_queue.Count == 0)
            {
                _showing = false;
                return;
            }

            _showing = true;
            var data = _queue.Dequeue();

            OnNotification?.Invoke(data);
            EventBus.Trigger("notificationShown", new() { ["title"] = data.Title });

            // Active object → wait out the duration on a coroutine. Otherwise (or duration <= 0)
            // drain synchronously so order is preserved without a frame delay (handy for tests).
            if (isActiveAndEnabled && data.Duration > 0f)
                StartCoroutine(AfterDelay(data.Duration));
            else
                ShowNext();
        }

        private IEnumerator AfterDelay(float duration)
        {
            yield return new WaitForSeconds(duration);
            ShowNext();
        }
    }
}
