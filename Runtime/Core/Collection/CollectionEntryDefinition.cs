using UnityEngine;
using ErccDev.Foundation.Core.Achievements;

namespace ErccDev.Foundation.Core.Collection
{
    /// <summary>
    /// Authoring asset for a single entry in the collection: display data + the rewards granted
    /// the first time it is discovered. The concrete content lives in each game's SO assets.
    /// Mirrors AchievementDefinition and reuses the same <see cref="Reward"/> assets, so the
    /// reward economy is shared between achievements and the collection.
    /// </summary>
    [CreateAssetMenu(menuName = "ErccDev/Collection/Entry")]
    public class CollectionEntryDefinition : ScriptableObject
    {
        [Header("Meta")]
        public string entryId;
        public bool   hidden;           // silhouette / secret until discovered

        [Header("Display")]
        public string title;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Rewards")]
        public Reward[] rewards;         // optional, granted on first discovery
    }
}
