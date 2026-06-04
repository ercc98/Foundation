using UnityEngine;
using ErccDev.Foundation.Core.Achievements;

namespace ErccDev.Foundation.Core.Collection
{
    /// <summary>
    /// Optional bridge that grants an entry's rewards when it is first discovered. Kept separate
    /// from CollectionManagerBase on purpose: the engine stays reward-agnostic, and games that
    /// don't want this connection simply don't add the component. It listens to the manager's
    /// <see cref="ICollectionService.OnDiscovered"/> event and reuses the shared Reward assets.
    /// </summary>
    public class CollectionRewardGranter : MonoBehaviour
    {
        [Header("Wiring")]
        [SerializeField] private MonoBehaviour collectionService;   // ICollectionService
        [SerializeField] private MonoBehaviour contextProvider;     // optional IAchievementContext

        private ICollectionService  _collection;
        private IAchievementContext _context;

        private void OnEnable()
        {
            _collection = collectionService as ICollectionService;
            _context    = contextProvider as IAchievementContext;

            if (_collection != null)
                _collection.OnDiscovered += Grant;
        }

        private void OnDisable()
        {
            if (_collection != null)
                _collection.OnDiscovered -= Grant;
        }

        private void Grant(CollectionEntryDefinition def)
        {
            if (def?.rewards == null) return;
            foreach (var r in def.rewards)
                r?.Grant(_context);
        }
    }
}
