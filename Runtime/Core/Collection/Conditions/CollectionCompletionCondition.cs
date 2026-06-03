using UnityEngine;
using ErccDev.Foundation.Core.Achievements;

namespace ErccDev.Foundation.Core.Collection.Conditions
{
    /// <summary>
    /// Achievement condition that completes when the collection reaches a target. Resolves the
    /// ICollectionService from the achievement context, so an achievement can read collection
    /// progress without the two managers referencing each other directly ("discover 50 entries",
    /// "complete the album"). Lives in the Collection module so Achievements stays unaware of it.
    /// </summary>
    [CreateAssetMenu(menuName = "ErccDev/Collection/Conditions/Collection Completion")]
    public class CollectionCompletionCondition : AchievementCondition
    {
        [Min(1)] public int targetCount = 1;

        [Tooltip("If true, requires the whole collection instead of targetCount.")]
        public bool requireFullCollection;

        private ICollectionService _collection;

        public override void Initialize(IAchievementContext context)
            => context?.TryGet(out _collection);

        public override bool IsCompleted()
        {
            if (_collection == null) return false;
            return _collection.DiscoveredCount >= Target;
        }

        public override float Progress01
            => _collection != null && Target > 0
                ? Mathf.Clamp01(_collection.DiscoveredCount / (float)Target)
                : 0f;

        public override void Cleanup() => _collection = null;

        private int Target
            => requireFullCollection && _collection != null ? _collection.TotalCount : targetCount;
    }
}
