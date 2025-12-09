using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Core.Save;

public class SaveServiceTests
{
    [Serializable]
    private class TestData
    {
        public string name;
        public int score;
    }

    private class TestSO : ScriptableObject
    {
        public string id;
        public int level;
    }

    private string _testFile;
    private string _persistentPath => Application.persistentDataPath;

    [SetUp]
    public void SetUp()
    {
        // unique file per test
        _testFile = Path.Combine(_persistentPath, $"test_{Guid.NewGuid()}.json");
        if (!Directory.Exists(_persistentPath))
            Directory.CreateDirectory(_persistentPath);
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var file in Directory.GetFiles(_persistentPath, "test_*.json*"))
            File.Delete(file);
    }

    // ---------------------------
    // Plain object tests
    // ---------------------------

    [Test]
    public void SaveObject_Creates_File_And_Can_Load_Back()
    {
        var data = new TestData { name = "Player", score = 99 };
        SaveService.SaveObject(data, Path.GetFileName(_testFile));

        Assert.IsTrue(File.Exists(_testFile), "File should exist after saving.");

        bool result = SaveService.TryLoadObject(Path.GetFileName(_testFile), out TestData loaded);
        Assert.IsTrue(result);
        Assert.AreEqual(data.name, loaded.name);
        Assert.AreEqual(data.score, loaded.score);
    }

    [Test]
    public void TryLoadObject_Returns_False_When_File_Not_Exist()
    {
        bool result = SaveService.TryLoadObject("nonexistent.json", out TestData data);
        Assert.IsFalse(result);
        Assert.IsNull(data);
    }

    // ---------------------------
    // ScriptableObject tests
    // ---------------------------

    [Test]
    public void SaveSO_Creates_File_And_LoadSO_Overwrites_Data()
    {
        var so = ScriptableObject.CreateInstance<TestSO>();
        so.id = "ABC";
        so.level = 5;

        SaveService.SaveSO(so, Path.GetFileName(_testFile));
        Assert.IsTrue(File.Exists(_testFile));

        // Change values before loading
        so.id = "Changed";
        so.level = 0;

        SaveService.LoadSO(so, Path.GetFileName(_testFile));

        Assert.AreEqual("ABC", so.id);
        Assert.AreEqual(5, so.level);
    }

    [Test]
    public void LoadSO_Creates_New_File_When_Not_Exist()
    {
        var so = ScriptableObject.CreateInstance<TestSO>();
        so.id = "Initial";
        so.level = 1;

        string fileName = Path.GetFileName(_testFile);
        string path = Path.Combine(_persistentPath, fileName);

        Assert.IsFalse(File.Exists(path));
        SaveService.LoadSO(so, fileName);
        Assert.IsTrue(File.Exists(path));
    }

    [Test]
    public void SaveAllSO_And_LoadAllSO_Works_For_Multiple_Objects()
    {
        var so1 = ScriptableObject.CreateInstance<TestSO>();
        var so2 = ScriptableObject.CreateInstance<TestSO>();
        so1.id = "One"; so1.level = 1;
        so2.id = "Two"; so2.level = 2;

        var list = new List<ScriptableObject> { so1, so2 };
        SaveService.SaveAllSO(list, Path.GetFileName(_testFile));
        Assert.IsTrue(File.Exists(_testFile));

        // mutate before loading
        so1.id = "X"; so2.id = "Y";

        SaveService.LoadAllSO(list, Path.GetFileName(_testFile));

        Assert.AreEqual("One", so1.id);
        Assert.AreEqual("Two", so2.id);
    }

    [Test]
    public void SaveSO_Does_Nothing_When_Null()
    {
        Assert.DoesNotThrow(() =>
        {
            SaveService.SaveSO<TestSO>(null, Path.GetFileName(_testFile));
        });
    }

    [Test]
    public void LoadAllSO_Does_Nothing_When_File_Not_Exist()
    {
        var so1 = ScriptableObject.CreateInstance<TestSO>();
        var list = new List<ScriptableObject> { so1 };
        Assert.DoesNotThrow(() => SaveService.LoadAllSO(list, "missing.json"));
    }
}