using System.Collections.Generic;
using UnityEngine;
using ErccDev.Foundation.Core.Events;
using ErccDev.Foundation.Core.Collection;

namespace ErccDev.Foundation.Core.Notifications.Sources
{
    /// <summary>
    /// Optional bridge: turns collection discoveries (and full completion) into notifications.
    /// Listens to the manager's typed OnDiscovered event for rich entry data, and to the
    /// "collectionCompleted" EventBus event for the completion toast. Drop-in and removable —
    /// neither system depends on it.
    /// </summary>
    public class CollectionNotificationSource : MonoBehaviour
    {
        [Header("Wiring")]
        [SerializeField] private MonoBehaviour collectionService;   // ICollectionService

        [Header("Display")]
        [SerializeField] private string discoveredHeadline = "New discovery";
        [SerializeField] private string completedMessage   = "Collection complete!";
        [SerializeField] private float  duration           = NotificationData.DefaultDuration;

        private ICollectionService _collection;

        private void OnEnable()
        {
            _collection = collectionService as ICollectionService;
            if (_collection != null)
                _collection.OnDiscovered += PushDiscovered;

            EventBus.StartListening("collectionCompleted", PushCompleted);
        }

        private void OnDisable()
        {
            if (_collection != null)
                _collection.OnDiscovered -= PushDiscovered;

            EventBus.StopListening("collectionCompleted", PushCompleted);
        }

        private void PushDiscovered(CollectionEntryDefinition def)
        {
            if (def == null) return;
            NotificationService.Notify(new NotificationData(
                title:    string.IsNullOrEmpty(def.title) ? discoveredHeadline : def.title,
                message:  def.description,
                icon:     def.icon,
                category: NotificationCategory.Collection,
                duration: duration));
        }

        private void PushCompleted(Dictionary<string, object> _)
        {
            NotificationService.Notify(new NotificationData(
                title:    completedMessage,
                category: NotificationCategory.Collection,
                duration: duration));
        }
    }
}
