using System.IO;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Core.Collection;

public class CollectionCatalogTests
{
    private readonly List<Object> _assets = new();
    private GameObject _go;
    private string _saveFile;

    // A subclass so Get<T> has something narrower than the base to filter on.
    private class SpecialEntry : CollectionEntryDefinition { }

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

    private T Entry<T>(string id) where T : CollectionEntryDefinition
    {
        var e = ScriptableObject.CreateInstance<T>();
        e.entryId = id;
        _assets.Add(e);
        return e;
    }

    // entries is serialized-private; populate it directly via reflection (edit-time equivalent).
    private CollectionCatalog Catalog(params CollectionEntryDefinition[] defs)
    {
        var cat = ScriptableObject.CreateInstance<CollectionCatalog>();
        typeof(CollectionCatalog)
            .GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(cat, new List<CollectionEntryDefinition>(defs));
        cat.Invalidate();
        _assets.Add(cat);
        return cat;
    }

    private CollectionProgressData Progress()
    {
        var p = ScriptableObject.CreateInstance<CollectionProgressData>();
        _assets.Add(p);
        return p;
    }

    // ---------- Catalog lookup ----------

    [Test]
    public void Get_Returns_Entry_By_Id_And_Null_On_Miss()
    {
        var a   = Entry<CollectionEntryDefinition>("a");
        var cat = Catalog(a, Entry<CollectionEntryDefinition>("b"));

        Assert.AreSame(a, cat.Get("a"));
        Assert.IsNull(cat.Get("zzz"));
        Assert.IsNull(cat.Get(null));
        Assert.AreEqual(2, cat.Count);
    }

    [Test]
    public void TryGet_Reports_Hit_And_Miss()
    {
        var a   = Entry<CollectionEntryDefinition>("a");
        var cat = Catalog(a);

        Assert.IsTrue(cat.TryGet("a", out var hit));
        Assert.AreSame(a, hit);

        Assert.IsFalse(cat.TryGet("nope", out var miss));
        Assert.IsNull(miss);
    }

    [Test]
    public void Generic_Get_Filters_By_Type()
    {
        var special = Entry<SpecialEntry>("special");
        var plain   = Entry<CollectionEntryDefinition>("plain");
        var cat     = Catalog(special, plain);

        Assert.AreSame(special, cat.Get<SpecialEntry>("special"));
        Assert.IsNull(cat.Get<SpecialEntry>("plain"), "wrong subtype filters out");
    }

    // ---------- Manager driven by the catalog ----------

    [Test]
    public void Manager_Drives_Counts_And_Discovery_From_The_Catalog()
    {
        var cat = Catalog(Entry<CollectionEntryDefinition>("a"),
                          Entry<CollectionEntryDefinition>("b"));

        _go = new GameObject("collection");
        var mgr = _go.AddComponent<TestableManager>();
        mgr.Configure(cat, Progress(), _saveFile);

        Assert.AreEqual(2, mgr.TotalCount, "TotalCount comes from the catalog");
        Assert.AreEqual(0f, mgr.Completion01);

        Assert.IsTrue(mgr.Discover("a"), "first discovery succeeds");
        Assert.IsFalse(mgr.Discover("a"), "idempotent: second discovery is a no-op");
        Assert.IsFalse(mgr.Discover("zzz"), "unknown id (not in catalog) is rejected");

        Assert.AreEqual(1, mgr.DiscoveredCount);
        Assert.AreEqual(0.5f, mgr.Completion01, 1e-4f);
    }

    private class TestableManager : CollectionManagerBase
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
}
