using UnityEngine;

namespace ErccDev.Foundation.Core.Achievements
{
    /// <summary>
    /// Base ScriptableObject reward — the "earn objects" half. Subclass per game to
    /// grant currency, items, skins, etc. Keep economy-specific logic in the subclass.
    /// </summary>
    public abstract class Reward : ScriptableObject, IReward
    {
        [Header("Meta")]
        public string rewardId;

        public abstract void Grant(IAchievementContext context);
    }
}
