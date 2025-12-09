using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Save
{
    /// <summary>
    /// JSON-based implementation of ISaveService.
    /// Writes/reads files under a base directory (by default Application.persistentDataPath).
    /// Supports ScriptableObjects (overwrite) and plain serializable objects.
    /// </summary>
    public class JsonFileSaveService : ISaveService
    {
        private readonly string _baseDirectory;

        /// <param name="baseDirectory">
        /// Optional base directory for all save files.
        /// If null or empty, uses Application.persistentDataPath.
        /// </param>
        public JsonFileSaveService(string baseDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
                _baseDirectory = Application.persistentDataPath;
            else
                _baseDirectory = baseDirectory;

            if (!Directory.Exists(_baseDirectory))
                Directory.CreateDirectory(_baseDirectory);
        }

        private string GetFilePath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("fileName cannot be null/empty.", nameof(fileName));

            return Path.Combine(_baseDirectory, fileName);
        }

        // ---------- ScriptableObject API ----------

        public void SaveSO<T>(T data, string fileName, bool pretty = true) where T : ScriptableObject
        {
            if (!data) return;
            string json = JsonUtility.ToJson(data, pretty);
            AtomicWrite(GetFilePath(fileName), json);
        }

        public void LoadSO<T>(T data, string fileName) where T : ScriptableObject
        {
            if (!data) return;
            string path = GetFilePath(fileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                JsonUtility.FromJsonOverwrite(json, data);
            }
            else
            {
                // If no file exists yet, save the current default state
                SaveSO(data, fileName);
            }
        }

        public void SaveAllSO(List<ScriptableObject> dataObjects, string fileName, bool pretty = true)
        {
            if (dataObjects == null || dataObjects.Count == 0) return;

            var payload = new JsonDataList { jsonData = new List<string>(dataObjects.Count) };
            foreach (var so in dataObjects)
            {
                if (!so)
                {
                    payload.jsonData.Add(null);
                    continue;
                }

                payload.jsonData.Add(JsonUtility.ToJson(so, pretty));
            }

            string json = JsonUtility.ToJson(payload, pretty);
            AtomicWrite(GetFilePath(fileName), json);
        }

        public void LoadAllSO(List<ScriptableObject> dataObjects, string fileName)
        {
            if (dataObjects == null || dataObjects.Count == 0) return;

            string path = GetFilePath(fileName);
            if (!File.Exists(path)) return;

            string json = File.ReadAllText(path);
            var payload = JsonUtility.FromJson<JsonDataList>(json);
            if (payload?.jsonData == null) return;

            for (int i = 0; i < dataObjects.Count && i < payload.jsonData.Count; i++)
            {
                if (!dataObjects[i] || string.IsNullOrEmpty(payload.jsonData[i])) continue;
                JsonUtility.FromJsonOverwrite(payload.jsonData[i], dataObjects[i]);
            }
        }

        // ---------- Plain object API ----------

        public void SaveObject<T>(T data, string fileName, bool pretty = true)
        {
            string json = JsonUtility.ToJson(data, pretty);
            AtomicWrite(GetFilePath(fileName), json);
        }

        public bool TryLoadObject<T>(string fileName, out T data)
        {
            string path = GetFilePath(fileName);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                data = JsonUtility.FromJson<T>(json);
                return true;
            }

            data = default;
            return false;
        }

        // ---------- Helpers ----------

        private void AtomicWrite(string path, string contents)
        {
            string tmp = path + ".tmp";
            File.WriteAllText(tmp, contents);

            if (File.Exists(path))
                File.Replace(tmp, path, null);
            else
                File.Move(tmp, path);
        }

        [Serializable]
        private class JsonDataList
        {
            public List<string> jsonData;
        }
    }
}