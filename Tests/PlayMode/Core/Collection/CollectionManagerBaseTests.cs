using System.IO;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Core.Collection;

public class CollectionManagerBaseTests
{
    // Test seam: skips the auto Awake so catalog/progress can be injected, then loads manually.
    private class TestableCollectionManager : CollectionManagerBase
    {
        protected override void Awake() { /* configured manually in tests */ }

        public void Configure(CollectionCatalog c, CollectionProgressData p, string file)
        {
            catalog      = c;
            progress     = p;
            saveFileName = file;
            LoadProgress();
        }
    }

    private string _saveFile;
    private GameObject _go;
    private readonly List<Object> _assets = new();

    [SetUp]
    public void SetUp()
    {
        _saveFile = $"test_collection_{System.Guid.NewGuid():N}.json";
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null) Object.DestroyImmediate(_go);
        foreach (var a in _assets) if (a != null) Object.DestroyImmediate(a);
        _assets.Clear();

        foreach (var file in Directory.GetFiles(Application.persistentDataPath, "test_collection_*.json*"))
            File.Delete(file);
    }

    private CollectionEntryDefinition Entry(string id)
    {
        var e = ScriptableObject.CreateInstance<CollectionEntryDefinition>();
        e.entryId = id;
        _assets.Add(e);
        return e;
    }

    private CollectionProgressData Progress()
    {
        var p = ScriptableObject.CreateInstance<CollectionProgressData>();
        _assets.Add(p);
        return p;
    }

    private CollectionCatalog Catalog(params string[] ids)
    {
        var entries = new List<CollectionEntryDefinition>();
        foreach (var id in ids) entries.Add(Entry(id));

        var cat = ScriptableObject.CreateInstance<CollectionCatalog>();
        // entries is serialized-private; populate it directly for the test.
        typeof(CollectionCatalog)
            .GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(cat, entries);
        cat.Invalidate();
        _assets.Add(cat);
        return cat;
    }

    private TestableCollectionManager Build(CollectionProgressData progress, params string[] ids)
    {
        _go = new GameObject("collection");
        var mgr = _go.AddComponent<TestableCollectionManager>();
        mgr.Configure(Catalog(ids), progress, _saveFile);
        return mgr;
    }

    [Test]
    public void Discover_Records_Unknown_Entry_Once()
    {
        var mgr = Build(Progress(), "a", "b", "c");

        Assert.IsTrue(mgr.Discover("a"), "first discovery returns true");
        Assert.IsFalse(mgr.Discover("a"), "re-discovering the same entry returns false");
        Assert.IsTrue(mgr.IsDiscovered("a"));
        Assert.AreEqual(1, mgr.DiscoveredCount);
    }

    [Test]
    public void Discover_Ignores_Unlisted_Entry()
    {
        var mgr = Build(Progress(), "a", "b");

        Assert.IsFalse(mgr.Discover("zzz"));
        Assert.AreEqual(0, mgr.DiscoveredCount);
    }

    [Test]
    public void OnDiscovered_Fires_Only_On_First_Discovery()
    {
        var mgr = Build(Progress(), "a");
        int fired = 0;
        mgr.OnDiscovered += _ => fired++;

        mgr.Discover("a");
        mgr.Discover("a");

        Assert.AreEqual(1, fired);
    }

    [Test]
    public void Completion01_Tracks_Ratio()
    {
        var mgr = Build(Progress(), "a", "b", "c", "d");
        Assert.AreEqual(0f, mgr.Completion01);

        mgr.Discover("a");
        mgr.Discover("b");

        Assert.AreEqual(0.5f, mgr.Completion01, 1e-4f);
        Assert.AreEqual(4, mgr.TotalCount);
    }

    [Test]
    public void Progress_Survives_A_Save_And_Reload()
    {
        var progress = Progress();
        var mgr = Build(progress, "a", "b");
        mgr.Discover("a");          // Discover() persists via the save system

        // Fresh manager + fresh progress SO reading the same file back.
        Object.DestroyImmediate(_go);
        var reloaded = Progress();
        var mgr2 = Build(reloaded, "a", "b");

        Assert.IsTrue(mgr2.IsDiscovered("a"));
        Assert.IsFalse(mgr2.IsDiscovered("b"));
        Assert.AreEqual(1, mgr2.DiscoveredCount);
    }
}
