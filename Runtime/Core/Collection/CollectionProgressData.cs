using System;
using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Collection
{
    /// <summary>
    /// Persistent progress for the collection — a ScriptableObject in the same spirit as
    /// SettingsData / PlayerProfileData, so it can be wired in the editor and saved through the
    /// same ISaveService used by the rest of the Foundation (JsonUtility overwrites the asset).
    /// Stores only what must survive between sessions: the ids that have been discovered.
    /// </summary>
    [CreateAssetMenu(fileName = "CollectionProgressData", menuName = "ErccDev/Collection/Collection Progress Data")]
    public sealed class CollectionProgressData : ScriptableObject
    {
        [Header("Discovered")]
        [SerializeField] private List<string> discoveredIds = new();

        // Fast lookup, rebuilt lazily from the serialized list after a load.
        private HashSet<string> _set;

        public IReadOnlyList<string> DiscoveredIds => discoveredIds;
        public int Count => discoveredIds.Count;

        public bool IsDiscovered(string entryId)
        {
            if (string.IsNullOrEmpty(entryId)) return false;
            EnsureSet();
            return _set.Contains(entryId);
        }

        /// <summary>Records an id. Returns true only when it was not present before.</summary>
        public bool Discover(string entryId)
        {
            if (string.IsNullOrEmpty(entryId)) return false;
            EnsureSet();
            if (!_set.Add(entryId)) return false;

            discoveredIds.Add(entryId);
            return true;
        }

        public void Clear()
        {
            discoveredIds.Clear();
            _set = null;
        }

        /// <summary>Drops the cached lookup so it rebuilds from the (freshly loaded) list.</summary>
        public void Invalidate() => _set = null;

        private void EnsureSet()
        {
            if (_set != null && _set.Count == discoveredIds.Count) return;
            _set = new HashSet<string>(discoveredIds, StringComparer.Ordinal);
        }
    }
}
