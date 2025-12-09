using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using ErccDev.Foundation.Loader;

public class SceneLoaderTests
{
    // Expose the protected IEnumerators for direct yielding in tests.
    private class TestableSceneLoader : SceneLoader
    {
        public IEnumerator CallUnloadByName(string name)   => base.UnloadSceneCoroutine(name);
        public IEnumerator CallLoadByName(string name, bool additive) => base.LoadSceneCoroutine(name, additive);
    }

    private GameObject _go;
    private TestableSceneLoader _loader;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("SceneLoader_GO");
        _loader = _go.AddComponent<TestableSceneLoader>();
        // Simulate Unity lifecycle
        _loader.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
    }

    [TearDown]
    public void TearDown()
    {
        if (_loader != null)
            Object.DestroyImmediate(_loader.gameObject);
        // Clean up any leftover test scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.isLoaded && s.name.StartsWith("TestScene_"))
                SceneManager.UnloadSceneAsync(s);
        }
    }

    // ---------- Tests ----------

    [UnityTest]
    public IEnumerator Awake_Sets_Singleton_And_Destroys_Duplicates()
    {
        // First instance becomes the singleton
        Assert.IsNotNull(SceneLoader.Instance);
        Assert.AreSame(_loader, SceneLoader.Instance);

        // Create a second GO+component; it should destroy itself in Awake
        var other = new GameObject("SceneLoader_GO_2").AddComponent<TestableSceneLoader>();
        other.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
        // Wait a frame for Destroy to process
        yield return null;

        // The singleton must remain the original
        Assert.AreSame(_loader, SceneLoader.Instance);
        Assert.IsTrue(other == null || other.Equals(null), "Duplicate SceneLoader should have been destroyed.");
    }

    [UnityTest]
    public IEnumerator IsThisScene_Returns_True_For_Active_Scene()
    {
        // Create a temporary scene and set it active
        var tempName = "TestScene_IsActive";
        var created = SceneManager.CreateScene(tempName);
        SceneManager.SetActiveScene(created);
        yield return null;

        Assert.IsTrue(_loader.IsThisScene(tempName));
        Assert.IsFalse(_loader.IsThisScene("SomeOtherScene"));
    }

    [UnityTest]
    public IEnumerator UnloadSceneCoroutine_ByName_Unloads_A_Runtime_Created_Scene()
    {
        // Arrange: create a loaded scene we can unload by name
        var tempName = "TestScene_Unload_ByName";
        var created = SceneManager.CreateScene(tempName);
        Assert.IsTrue(created.isLoaded);
        yield return null;

        // Act: call the protected coroutine via the testable wrapper
        yield return _loader.CallUnloadByName(tempName);

        // Assert: scene should be gone (not loaded)
        var after = SceneManager.GetSceneByName(tempName);
        Assert.IsFalse(after.isLoaded, "Scene should have been unloaded by name.");
    }
    
    // Optional: quick smoke test for public API forwarding
    [UnityTest]
    public IEnumerator Public_UnloadSceneAsync_Forwards_To_Coroutine()
    {
        var tempName = "TestScene_Public_Unload";
        var created = SceneManager.CreateScene(tempName);
        Assert.IsTrue(created.isLoaded);
        yield return null;

        // Call the public wrapper
        _loader.UnloadSceneAsync(tempName);
        // Wait a few frames for coroutine to finish
        yield return null;
        yield return null;
        yield return null;

        Assert.IsFalse(SceneManager.GetSceneByName(tempName).isLoaded);
    }
}