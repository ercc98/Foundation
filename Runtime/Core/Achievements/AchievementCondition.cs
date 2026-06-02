using UnityEngine;

namespace ErccDev.Foundation.Core.Achievements
{
    /// <summary>
    /// Base ScriptableObject condition. Subclass per game with concrete rules
    /// (e.g. "collect N coins", "reach level X"). Keep game-specific logic here.
    /// </summary>
    public abstract class AchievementCondition : ScriptableObject, IAchievementCondition
    {
        public abstract void  Initialize(IAchievementContext context);
        public abstract bool  IsCompleted();
        public virtual  float Progress01 => IsCompleted() ? 1f : 0f;
        public abstract void  Cleanup();
    }
}
