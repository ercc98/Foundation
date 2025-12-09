using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Save
{
    /// <summary>
    /// Static facade over an ISaveService (JsonFileSaveService by default).
    /// Allows quick static calls but keeps the system extensible and testable.
    /// </summary>
    public static class SaveService
    {
        private static ISaveService _default = new JsonFileSaveService();

        /// <summary>
        /// Current default save service (JsonFileSaveService by default).
        /// </summary>
        public static ISaveService Default => _default;

        /// <summary>
        /// Allows swapping the implementation (for tests, encryption, cloud, etc).
        /// </summary>
        public static void SetDefault(ISaveService customService)
        {
            if (customService != null)
                _default = customService;
        }

        // ---------- ScriptableObject API ----------

        public static void SaveSO<T>(T data, string fileName, bool pretty = true) where T : ScriptableObject
            => _default.SaveSO(data, fileName, pretty);

        public static void LoadSO<T>(T data, string fileName) where T : ScriptableObject
            => _default.LoadSO(data, fileName);

        public static void SaveAllSO(List<ScriptableObject> dataObjects, string fileName, bool pretty = true)
            => _default.SaveAllSO(dataObjects, fileName, pretty);

        public static void LoadAllSO(List<ScriptableObject> dataObjects, string fileName)
            => _default.LoadAllSO(dataObjects, fileName);

        // ---------- Plain object API ----------

        public static void SaveObject<T>(T data, string fileName, bool pretty = true)
            => _default.SaveObject(data, fileName, pretty);

        public static bool TryLoadObject<T>(string fileName, out T data)
            => _default.TryLoadObject(fileName, out data);
    }
}