using System;
using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Collection
{
    /// <summary>
    /// One authored list of every collection entry the game knows about — the single source of truth
    /// shared by the manager and any UI that lists/looks up entries, so nobody keeps a drifting copy.
    /// Holds the base type, so it carries all reward kinds; consumers filter via <see cref="Get{T}"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "ErccDev/Collection/Catalog", fileName = "CollectionCatalog")]
    public sealed class CollectionCatalog : ScriptableObject
    {
        [SerializeField] private List<CollectionEntryDefinition> entries = new();

        // Fast id lookup, rebuilt lazily from the serialized list.
        private Dictionary<string, CollectionEntryDefinition> _byId;

        public IReadOnlyList<CollectionEntryDefinition> Entries => entries;
        public int Count => entries.Count;

        public CollectionEntryDefinition Get(string entryId)
        {
            if (string.IsNullOrEmpty(entryId)) return null;
            EnsureMap();
            return _byId.TryGetValue(entryId, out var def) ? def : null;
        }

        public bool TryGet(string entryId, out CollectionEntryDefinition def)
            => (def = Get(entryId)) != null;

        public T Get<T>(string entryId) where T : CollectionEntryDefinition => Get(entryId) as T;

        /// <summary>Drops the cached lookup so it rebuilds from the (freshly edited) list.</summary>
        public void Invalidate() => _byId = null;

        private void EnsureMap()
        {
            if (_byId != null && _byId.Count == entries.Count) return;
            _byId = new Dictionary<string, CollectionEntryDefinition>(StringComparer.Ordinal);
            foreach (var e in entries)
                if (e != null && !string.IsNullOrEmpty(e.entryId)) _byId[e.entryId] = e;
        }

#if UNITY_EDITOR
        /// <summary>Rebuilds from every entry asset in the project, sorted by id. Editor-only; result is serialized.</summary>
        [ContextMenu("Refresh From Project")]
        public void RefreshFromProject()
        {
            entries.Clear();
            foreach (var guid in UnityEditor.AssetDatabase.FindAssets("t:CollectionEntryDefinition"))
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var def  = UnityEditor.AssetDatabase.LoadAssetAtPath<CollectionEntryDefinition>(path);
                if (def != null) entries.Add(def);
            }
            entries.Sort((a, b) => string.CompareOrdinal(a != null ? a.entryId : "", b != null ? b.entryId : ""));
            Invalidate();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
