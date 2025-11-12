using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Audio
{
    [CreateAssetMenu(menuName = "ErccDev/Audio/Sound Group")]
    public class SoundGroup : ScriptableObject
    {
        public SoundEntry[] entries;
        private Dictionary<string, SoundEntry> _map;

        public void Init()
        {
            if (_map != null) return;
            _map = new Dictionary<string, SoundEntry>(entries?.Length ?? 0);
            if (entries == null) return;
            foreach (var e in entries)
            {
                if (e != null && !string.IsNullOrEmpty(e.id) && !_map.ContainsKey(e.id))
                    _map.Add(e.id, e);
            }
        }

        public SoundEntry Get(string id)
        {
            if (_map == null) Init();
            return (id != null && _map != null && _map.TryGetValue(id, out var e)) ? e : null;
        }
    }
}