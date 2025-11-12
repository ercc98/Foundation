using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace ErccDev.Foundation.Loader
{
    /// <summary>
    /// Reusable scene loader supporting both SceneManager and Addressables.
    /// One-per-project singleton; survives scene loads.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        // Track Addressables scene handles so we can unload by reference or key safely
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _addressableHandles = new();

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        // ---------- Public API ----------
        public virtual void LoadSceneAsync(string sceneName, bool additive = false)
            => StartCoroutine(LoadSceneCoroutine(sceneName, additive));

        public virtual void LoadSceneAsync(AssetReference sceneAssetReference, bool additive = false)
            => StartCoroutine(LoadSceneCoroutine(sceneAssetReference, additive));

        public virtual void UnloadSceneAsync(string sceneName)
            => StartCoroutine(UnloadSceneCoroutine(sceneName));

        public virtual void UnloadSceneAsync(AssetReference sceneAssetReference)
            => StartCoroutine(UnloadSceneCoroutine(sceneAssetReference));

        public virtual bool IsThisScene(string sceneName)
            => SceneManager.GetActiveScene().name == sceneName;

        // ---------- Coroutines ----------
        protected virtual IEnumerator LoadSceneCoroutine(string sceneName, bool additive = false)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(
                sceneName,
                additive ? LoadSceneMode.Additive : LoadSceneMode.Single
            );
            while (!op.isDone) yield return null;
        }

        protected virtual IEnumerator LoadSceneCoroutine(AssetReference sceneAssetReference, bool additive = false)
        {
            var handle = sceneAssetReference.LoadSceneAsync(
                additive ? LoadSceneMode.Additive : LoadSceneMode.Single,
                activateOnLoad: true
            );

            // Use RuntimeKey as dictionary key (string)
            string key = sceneAssetReference.RuntimeKey.ToString();
            _addressableHandles[key] = handle;

            while (!handle.IsDone) yield return null;
        }

        protected virtual IEnumerator UnloadSceneCoroutine(string sceneName)
        {
            // If a non-addressable scene with this name is loaded, unload it
            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);
                while (!op.isDone) yield return null;
                yield break;
            }

            // Fallback: if we tracked an addressable with this key/name, unload via Addressables
            if (_addressableHandles.TryGetValue(sceneName, out var handleByName))
            {
                var unloadHandle = Addressables.UnloadSceneAsync(handleByName, true);
                while (!unloadHandle.IsDone) yield return null;
                _addressableHandles.Remove(sceneName);
            }
        }

        protected virtual IEnumerator UnloadSceneCoroutine(AssetReference sceneAssetReference)
        {
            string key = sceneAssetReference.RuntimeKey.ToString();
            if (_addressableHandles.TryGetValue(key, out var handle))
            {
                var unloadHandle = Addressables.UnloadSceneAsync(handle, true);
                while (!unloadHandle.IsDone) yield return null;
                _addressableHandles.Remove(key);
            }
        }
    }
}
