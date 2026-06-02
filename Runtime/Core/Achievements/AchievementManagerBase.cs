using System;
using System.Collections.Generic;
using UnityEngine;
using ErccDev.Foundation.Core.Events;
using ErccDev.Foundation.Core.Save;

namespace ErccDev.Foundation.Core.Achievements
{
    /// <summary>
    /// Generic achievement engine: tracks progress, unlocks, persists, and grants rewards.
    /// All game content lives in the AchievementDefinition / Condition / Reward assets — this
    /// base only orchestrates them. Subclass to add custom popups, analytics, etc.
    ///
    /// Call Evaluate() when something relevant happens (or subscribe it to your gameplay
    /// EventBus events) rather than polling every frame.
    /// </summary>
    public class AchievementManagerBase : MonoBehaviour, IAchievementService
    {
        [Header("Setup")]
        [SerializeField] protected List<AchievementDefinition> achievements = new();
        [SerializeField] protected MonoBehaviour contextProvider;          // optional IAchievementContext
        [SerializeField] protected string saveFileName = "achievements";

        protected readonly HashSet<string> _unlocked = new(StringComparer.Ordinal);
        protected IAchievementContext Context { get; private set; }

        public event Action<AchievementDefinition> OnUnlocked;

        // ---------- Lifecycle ----------

        protected virtual void Awake()
        {
            Context = contextProvider as IAchievementContext;
            LoadProgress();

            foreach (var a in achievements)
                if (a != null && !IsUnlocked(a.achievementId))
                    a.Initialize(Context);
        }

        protected virtual void OnDestroy()
        {
            foreach (var a in achievements)
                a?.Cleanup();
        }

        // ---------- IAchievementService ----------

        public bool IsUnlocked(string achievementId)
            => achievementId != null && _unlocked.Contains(achievementId);

        public float GetProgress01(string achievementId)
        {
            if (IsUnlocked(achievementId)) return 1f;
            var def = Find(achievementId);
            return def != null ? def.Progress01 : 0f;
        }

        public virtual void Evaluate()
        {
            for (int i = 0; i < achievements.Count; i++)
            {
                var a = achievements[i];
                if (a == null || IsUnlocked(a.achievementId)) continue;
                if (a.IsCompleted()) Unlock(a);
            }
        }

        // ---------- Unlock / rewards ----------

        protected virtual void Unlock(AchievementDefinition def)
        {
            if (def == null || !_unlocked.Add(def.achievementId)) return;

            def.Cleanup();
            GrantRewards(def);
            SaveProgress();

            OnUnlocked?.Invoke(def);
            EventBus.Trigger("achievementUnlocked", new() { ["id"] = def.achievementId });
        }

        protected virtual void GrantRewards(AchievementDefinition def)
        {
            if (def.rewards == null) return;
            foreach (var r in def.rewards)
                r?.Grant(Context);
        }

        // ---------- Persistence ----------

        protected virtual void LoadProgress()
        {
            if (SaveService.TryLoadObject<AchievementProgress>(saveFileName, out var data) && data?.unlockedIds != null)
                foreach (var id in data.unlockedIds)
                    _unlocked.Add(id);
        }

        protected virtual void SaveProgress()
        {
            var data = new AchievementProgress { unlockedIds = new List<string>(_unlocked) };
            SaveService.SaveObject(data, saveFileName);
        }

        // ---------- Helpers ----------

        protected AchievementDefinition Find(string id)
        {
            for (int i = 0; i < achievements.Count; i++)
                if (achievements[i] != null && achievements[i].achievementId == id)
                    return achievements[i];
            return null;
        }
    }
}
