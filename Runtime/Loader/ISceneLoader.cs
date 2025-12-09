using UnityEngine.AddressableAssets;

namespace ErccDev.Foundation.Loader
{
    /// <summary>
    /// Abstraction for loading and unloading scenes, with support for both
    /// standard build scenes and Addressables scene references.
    /// </summary>
    public interface ISceneLoader
    {
        void LoadSceneAsync(string sceneName, bool additive = false);

        void LoadSceneAsync(AssetReference sceneAssetReference, bool additive = false);

        void UnloadSceneAsync(string sceneName);

        void UnloadSceneAsync(AssetReference sceneAssetReference);

        bool IsThisScene(string sceneName);
    }
}