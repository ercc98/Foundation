using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Save
{
    public interface ISaveService
    {
        // ScriptableObject API
        void SaveSO<T>(T data, string fileName, bool pretty = true) where T : ScriptableObject;
        void LoadSO<T>(T data, string fileName) where T : ScriptableObject;

        void SaveAllSO(List<ScriptableObject> dataObjects, string fileName, bool pretty = true);
        void LoadAllSO(List<ScriptableObject> dataObjects, string fileName);

        // Plain object API
        void SaveObject<T>(T data, string fileName, bool pretty = true);
        bool TryLoadObject<T>(string fileName, out T data);
    }
}