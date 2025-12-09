using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Core.Pooling;

public class ObjectPoolTests
{
    private GameObject _prefab;
    private Transform _parent;
    private ObjectPool<TestComponent> _pool;

    // A simple test component for pooling
    private class TestComponent : MonoBehaviour { }

    [SetUp]
    public void SetUp()
    {
        _prefab = new GameObject("Prefab", typeof(TestComponent));
        _parent = new GameObject("Parent").transform;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_prefab);
        Object.DestroyImmediate(_parent.gameObject);
    }

    [Test]
    public void Pool_Initializes_With_Correct_Size()
    {
        _pool = new ObjectPool<TestComponent>(_prefab.GetComponent<TestComponent>(), 5, _parent);
        // Expect 5 inactive objects under parent
        Assert.AreEqual(5, _parent.childCount);
        foreach (Transform child in _parent)
        {
            Assert.IsFalse(child.gameObject.activeSelf);
        }
    }

    [Test]
    public void Get_Returns_An_Object_From_Pool()
    {
        _pool = new ObjectPool<TestComponent>(_prefab.GetComponent<TestComponent>(), 1, _parent);
        var obj = _pool.Get();

        Assert.IsNotNull(obj);
        Assert.IsInstanceOf<TestComponent>(obj);
    }

    [Test]
    public void Get_When_Empty_Creates_New_Object()
    {
        _pool = new ObjectPool<TestComponent>(_prefab.GetComponent<TestComponent>(), 0, _parent);
        var obj = _pool.Get();

        Assert.IsNotNull(obj);
        Assert.AreEqual(1, _parent.childCount);
    }

    [Test]
    public void Release_Adds_Object_Back_To_Pool()
    {
        _pool = new ObjectPool<TestComponent>(_prefab.GetComponent<TestComponent>(), 0, _parent);
        var obj = _pool.Get();

        _pool.Release(obj);

        // Getting again should return the same object
        var reusedObj = _pool.Get();
        Assert.AreSame(obj, reusedObj);
    }

    [Test]
    public void Released_Object_Is_Inactive()
    {
        _pool = new ObjectPool<TestComponent>(_prefab.GetComponent<TestComponent>(), 0, _parent);
        var obj = _pool.Get();

        obj.gameObject.SetActive(true);
        _pool.Release(obj);

        Assert.IsFalse(obj.gameObject.activeSelf);
    }
}