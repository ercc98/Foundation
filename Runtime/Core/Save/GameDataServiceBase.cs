using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Save
{
    public abstract class GameDataServiceBase : MonoBehaviour
    {
        [Header("Persistence")]
        [SerializeField] private string fileName = "playerdata.json";

        [Tooltip("Load automatically on Awake.")]
        [SerializeField] private bool loadOnAwake = true;

        [Tooltip("Autosave when app goes to background (mobile).")]
        [SerializeField] private bool saveOnPause = true;

        [Tooltip("Autosave when app quits (PC/Editor).")]
        [SerializeField] private bool saveOnQuit = true;

        private static GameDataServiceBase _instance;
        private List<ScriptableObject> _cachedObjects;

        /// <summary>Derived classes provide the list of ScriptableObjects to persist.</summary>
        protected abstract List<ScriptableObject> BuildObjects();

        /// <summary>Allow derived classes to access the file name if needed.</summary>
        protected string FileName => fileName;

        protected virtual void Awake()
        {
            // Singleton guard (prevents duplicates if multiple scenes include this object)
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            DontDestroyOnLoad(gameObject);

            if (loadOnAwake)
                LoadAll();
        }

        protected List<ScriptableObject> GetObjectsCached()
        {
            if (_cachedObjects == null)
            {
                _cachedObjects = BuildObjects();

                // Defensive cleanup (optional)
                if (_cachedObjects != null)
                    _cachedObjects.RemoveAll(o => o == null);
            }

            return _cachedObjects;
        }

        /// <summary>Call this if the set of objects changes at runtime.</summary>
        protected void InvalidateCache()
        {
            _cachedObjects = null;
        }

        public void SaveAll(bool pretty = true)
        {
            var objects = GetObjectsCached();
            if (objects == null || objects.Count == 0) return;

            SaveService.SaveAllSO(objects, fileName, pretty);
        }

        public void LoadAll()
        {
            var objects = GetObjectsCached();
            if (objects == null || objects.Count == 0) return;

            SaveService.LoadAllSO(objects, fileName);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause && saveOnPause)
                SaveAll();
        }

        private void OnApplicationQuit()
        {
            if (saveOnQuit)
                SaveAll();
        }
    }
}
