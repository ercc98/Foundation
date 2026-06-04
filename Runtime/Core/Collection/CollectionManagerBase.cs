using System;
using System.Collections.Generic;
using UnityEngine;
using ErccDev.Foundation.Core.Events;
using ErccDev.Foundation.Core.Save;

namespace ErccDev.Foundation.Core.Collection
{
    /// <summary>
    /// Generic collection engine: tracks which entries have been discovered, persists the
    /// progress ScriptableObject, and broadcasts the discovery. All content lives in the
    /// CollectionEntryDefinition assets — this base only orchestrates them. Subclass to add
    /// custom popups, analytics, etc.
    ///
    /// Deliberately knows nothing about rewards: it exposes <see cref="OnDiscovered"/> and fires
    /// "collectionEntryDiscovered" / "collectionCompleted" on the EventBus. Side effects such as
    /// granting rewards are wired separately (see CollectionRewardGranter) or by listening to
    /// those events, so the engine stays single-responsibility and reward-economy agnostic.
    /// </summary>
    public class CollectionManagerBase : MonoBehaviour, ICollectionService
    {
        [Header("Setup")]
        [SerializeField] protected List<CollectionEntryDefinition> entries = new();
        [SerializeField] protected CollectionProgressData progress;
        [SerializeField] protected string saveFileName = "collection.json";

        public event Action<CollectionEntryDefinition> OnDiscovered;

        // ---------- Lifecycle ----------

        protected virtual void Awake()
        {
            LoadProgress();
        }

        // ---------- ICollectionService ----------

        public int   TotalCount      => entries.Count;
        public int   DiscoveredCount => progress != null ? progress.Count : 0;
        public float Completion01    => entries.Count > 0 ? Mathf.Clamp01(DiscoveredCount / (float)entries.Count) : 0f;

        public bool IsDiscovered(string entryId)
            => progress != null && progress.IsDiscovered(entryId);

        public virtual bool Discover(string entryId)
        {
            var def = Find(entryId);
            if (def == null || progress == null) return false;
            if (!progress.Discover(entryId)) return false;     // already discovered

            SaveProgress();

            OnDiscovered?.Invoke(def);
            EventBus.Trigger("collectionEntryDiscovered", new() { ["id"] = entryId });

            if (DiscoveredCount >= entries.Count)
                EventBus.Trigger("collectionCompleted", new() { ["count"] = DiscoveredCount });

            return true;
        }

        // ---------- Persistence ----------

        protected virtual void LoadProgress()
        {
            if (progress == null) return;
            SaveService.LoadSO(progress, saveFileName);
            progress.Invalidate();
        }

        protected virtual void SaveProgress()
        {
            if (progress == null) return;
            SaveService.SaveSO(progress, saveFileName);
        }

        // ---------- Helpers ----------

        protected CollectionEntryDefinition Find(string entryId)
        {
            if (string.IsNullOrEmpty(entryId)) return null;
            for (int i = 0; i < entries.Count; i++)
                if (entries[i] != null && entries[i].entryId == entryId)
                    return entries[i];
            return null;
        }
    }
}
