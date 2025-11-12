using System.Collections;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Core.Pooling;
using ErccDev.Foundation.Core.Factories;

public class FactoryTests
{
    // Simple component to spawn
    private class TestComponent : MonoBehaviour { }

    // Concrete wrapper so we can test the abstract Factory<T>
    private class TestFactory : Factory<TestComponent>
    {
        public ObjectPool<TestComponent> Pool => _pool;
        public void CallAwake() => base.Awake(); // expose protected Awake
    }

    private GameObject _root;
    private TestFactory _factory;
    private TestComponent _prefab;

    [SetUp]
    public void SetUp()
    {
        _root = new GameObject("FactoryRoot");
        _factory = _root.AddComponent<TestFactory>();

        var prefabGO = new GameObject("Prefab", typeof(TestComponent));
        _prefab = prefabGO.GetComponent<TestComponent>();

        // Assign prefab via serialized field
        _factory.prefab = _prefab;
    }

    [TearDown]
    public void TearDown()
    {
        if (_factory) Object.DestroyImmediate(_factory.gameObject);
        if (_prefab) Object.DestroyImmediate(_prefab.gameObject);
    }

    [Test]
    public void Awake_WarmsPool_WhenPrefabAssigned()
    {
        _factory.CallAwake();

        // Default initialPoolSize is 10
        Assert.NotNull(_factory.Pool);
        Assert.AreEqual(10, _factory.transform.childCount);

        foreach (Transform child in _factory.transform)
            Assert.IsFalse(child.gameObject.activeSelf);
    }

    [Test]
    public void WarmPool_Is_Idempotent()
    {
        _factory.CallAwake();
        var poolRef = _factory.Pool;
        var childCount = _factory.transform.childCount;

        _factory.WarmPool();

        Assert.AreSame(poolRef, _factory.Pool);
        Assert.AreEqual(childCount, _factory.transform.childCount);
    }

    [Test]
    public void Spawn_Activates_And_Sets_Pose()
    {
        _factory.CallAwake();

        var pos = new Vector3(1, 2, 3);
        var rot = Quaternion.Euler(10, 20, 30);

        var instance = _factory.Spawn(pos, rot);

        Assert.NotNull(instance);
        Assert.IsTrue(instance.gameObject.activeSelf);
        Assert.AreEqual(pos, instance.transform.position);
        Assert.That(Quaternion.Angle(rot, instance.transform.rotation), Is.LessThan(0.0001f));
        Assert.AreSame(_factory.transform, instance.transform.parent);
    }

    [Test]
    public void Spawn_WarmsPool_When_NotInitialized()
    {
        // Do NOT call Awake()
        var pos = Vector3.zero;
        var rot = Quaternion.identity;

        var instance = _factory.Spawn(pos, rot);

        Assert.NotNull(_factory.Pool);
        Assert.NotNull(instance);
        Assert.IsTrue(instance.gameObject.activeSelf);
    }

    [Test]
    public void Recycle_Deactivates_And_Reuses_Instance()
    {
        _factory.CallAwake();

        var first = _factory.Spawn(Vector3.zero, Quaternion.identity);
        _factory.Spawn(Vector3.zero, Quaternion.identity);
        _factory.Spawn(Vector3.zero, Quaternion.identity);
        _factory.Spawn(Vector3.zero, Quaternion.identity);
        _factory.Spawn(Vector3.zero, Quaternion.identity);
        _factory.Spawn(Vector3.zero, Quaternion.identity);
        _factory.Spawn(Vector3.zero, Quaternion.identity);
        _factory.Spawn(Vector3.zero, Quaternion.identity);
        _factory.Spawn(Vector3.zero, Quaternion.identity);
        _factory.Spawn(Vector3.zero, Quaternion.identity);
        Assert.IsTrue(first.gameObject.activeSelf);

        _factory.Recycle(first);
        Assert.IsFalse(first.gameObject.activeSelf);

        var second = _factory.Spawn(Vector3.zero, Quaternion.identity);
        Assert.AreSame(first, second, "Factory should reuse recycled instance.");
    }

    [Test]
    public void Recycle_Is_Safe_When_Pool_Not_Warmed()
    {
        // No Awake(), no Spawn()
        // Should not throw when pool is null
        Assert.DoesNotThrow(() => _factory.Recycle((TestComponent)null));
    }

    [Test]
    public void WarmPool_With_Null_Prefab_Does_Not_Create_Pool()
    {
        // Use a new factory with null prefab
        var go = new GameObject("NoPrefabRoot");
        var f2 = go.AddComponent<TestFactory>();
        f2.prefab = null;

        f2.CallAwake();

        Assert.IsNull(f2.Pool);
        Assert.AreEqual(0, f2.transform.childCount);

        Object.DestroyImmediate(go);
    }
}