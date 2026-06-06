using UnityEngine;
using ErccDev.Foundation.Core.Achievements;

namespace ErccDev.Foundation.Core.Collection
{
    /// <summary>
    /// Marks a collection entry as discovered when an achievement is earned — the achievement-side
    /// counterpart to discovering it in-world. Pulls <see cref="ICollectionService"/> from the shared
    /// context, so it never references a concrete manager. <c>Discover</c> is idempotent.
    /// </summary>
    [CreateAssetMenu(menuName = "ErccDev/Collection/Rewards/Unlock Collection Entry")]
    public sealed class UnlockCollectionEntryReward : Reward
    {
        [Header("Collection")]
        [Tooltip("Entry id to discover. Falls back to rewardId when left empty.")]
        [SerializeField] private string entryId;

        public override void Grant(IAchievementContext context)
        {
            if (context == null || !context.TryGet<ICollectionService>(out var collection)) return;

            var id = string.IsNullOrEmpty(entryId) ? rewardId : entryId;
            collection.Discover(id);   // idempotent; persists + fires events via the manager
        }
    }
}
