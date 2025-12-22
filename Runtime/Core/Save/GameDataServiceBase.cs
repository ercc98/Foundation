using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Save
{
    public abstract class GameDataServiceBase : MonoBehaviour
    {
        [Header("Save")]
        [SerializeField] private bool loadOnAwake = true;

        protected abstract string FileName { get; }
        protected abstract List<ScriptableObject> BuildObjects();

        private List<ScriptableObject> _cachedObjects;

        protected virtual void Awake()
        {
            if (loadOnAwake)
                LoadAll();
        }

        protected List<ScriptableObject> GetObjectsCached()
        {
            if (_cachedObjects == null)
            {
                _cachedObjects = BuildObjects();

                // Defensive cleanup (optional but recommended)
                if (_cachedObjects != null)
                {
                    _cachedObjects.RemoveAll(o => o == null);
                }
            }

            return _cachedObjects;
        }

        public void SaveAll(bool pretty = true)
        {
            var objects = GetObjectsCached();
            if (objects == null || objects.Count == 0) return;

            SaveService.SaveAllSO(objects, FileName, pretty);
        }

        public void LoadAll()
        {
            var objects = GetObjectsCached();
            if (objects == null || objects.Count == 0) return;

            SaveService.LoadAllSO(objects, FileName);
        }

        protected virtual void OnApplicationPause(bool pause)
        {
            if (pause) SaveAll();
        }

        protected virtual void OnApplicationQuit()
        {
            SaveAll();
        }
    }
}
