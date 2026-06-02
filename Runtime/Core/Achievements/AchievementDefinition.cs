using UnityEngine;

namespace ErccDev.Foundation.Core.Achievements
{
    /// <summary>
    /// Authoring asset for a single achievement: display data + the condition that unlocks
    /// it + the rewards it grants. The concrete content lives in each game's SO assets.
    /// </summary>
    [CreateAssetMenu(menuName = "ErccDev/Achievements/Achievement")]
    public class AchievementDefinition : ScriptableObject
    {
        [Header("Meta")]
        public string achievementId;
        public bool   hidden;           // secret until unlocked

        [Header("Display")]
        public string title;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Behavior")]
        public AchievementCondition condition;
        public Reward[]             rewards;

        public void  Initialize(IAchievementContext context) => condition?.Initialize(context);
        public bool  IsCompleted() => condition != null && condition.IsCompleted();
        public float Progress01    => condition != null ? Mathf.Clamp01(condition.Progress01) : 0f;
        public void  Cleanup()     => condition?.Cleanup();
    }
}
