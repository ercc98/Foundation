using System;
using System.Collections.Generic;
using UnityEngine;
using ErccDev.Foundation.Core.Events;
using ErccDev.Foundation.Core.Save;
using ErccDev.Foundation.Core.Achievements;

namespace ErccDev.Foundation.Core.Collection
{
    /// <summary>
    /// Generic collection engine: tracks which entries have been discovered, persists the
    /// progress ScriptableObject, grants the entry's rewards on first discovery, and broadcasts
    /// EventBus events. All content lives in the CollectionEntryDefinition / Reward assets — this
    /// base only orchestrates them. Subclass to add custom popups, analytics, etc.
    ///
    /// Connections:
    ///   • Rewards    — entries carry the same Reward assets as achievements, granted via context.
    ///   • Achievements — fires "collectionEntryDiscovered" / "collectionCompleted" so an
    ///                    EventCountCondition (or CollectionCompletionCondition) can react.
    /// </summary>
    public class CollectionManagerBase : MonoBehaviour, ICollectionService
    {
        [Header("Setup")]
        [SerializeField] protected List<CollectionEntryDefinition> entries = new();
        [SerializeField] protected CollectionProgressData progress;
        [SerializeField] protected MonoBehaviour contextProvider;          // optional IAchievementContext (for rewards)
        [SerializeField] protected string saveFileName = "collection.json";

        protected IAchievementContext Context { get; private set; }

        public event Action<CollectionEntryDefinition> OnDiscovered;

        // ---------- Lifecycle ----------

        protected virtual void Awake()
        {
            Context = contextProvider as IAchievementContext;
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

            GrantRewards(def);
            SaveProgress();

            OnDiscovered?.Invoke(def);
            EventBus.Trigger("collectionEntryDiscovered", new() { ["id"] = entryId });

            if (DiscoveredCount >= entries.Count)
                EventBus.Trigger("collectionCompleted", new() { ["count"] = DiscoveredCount });

            return true;
        }

        // ---------- Rewards ----------

        protected virtual void GrantRewards(CollectionEntryDefinition def)
        {
            if (def.rewards == null) return;
            foreach (var r in def.rewards)
                r?.Grant(Context);
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
