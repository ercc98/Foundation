using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Core.Input;

public class InputModuleTests
{
    // ---- Test doubles: a concrete module over a throwaway config SO ----

    private class StubConfig : ScriptableObject { }

    private class StubInputModule : InputModule<StubConfig>
    {
        public int      EnableCount;
        public int      DisableCount;
        public int      AfterChangeCount;
        public int      BeforeChangeCount;
        public StubConfig LastOld;
        public StubConfig LastNew;

        public override void EnableModule()  => EnableCount++;
        public override void DisableModule() => DisableCount++;

        protected override void OnAfterConfigChange() => AfterChangeCount++;

        protected override void OnBeforeConfigChange(StubConfig oldConfig, StubConfig newConfig)
        {
            BeforeChangeCount++;
            LastOld = oldConfig;
            LastNew = newConfig;
        }
    }

    private GameObject _go;
    private StubInputModule _module;
    private StubConfig _config;

    [SetUp]
    public void SetUp()
    {
        _go     = new GameObject("InputModule_Test");
        _module = _go.AddComponent<StubInputModule>(); // active GO => Awake + OnEnable run
        _config = ScriptableObject.CreateInstance<StubConfig>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)     UnityEngine.Object.DestroyImmediate(_go);
        if (_config != null) UnityEngine.Object.DestroyImmediate(_config);
    }

    [Test]
    public void OnEnable_EnablesModuleOnce()
    {
        Assert.AreEqual(1, _module.EnableCount, "OnEnable should call EnableModule exactly once.");
    }

    [Test]
    public void SettingConfig_RaisesBeforeAndAfterChange_AndStoresValue()
    {
        _module.Config = _config;

        Assert.AreSame(_config, _module.Config);
        Assert.AreEqual(1, _module.BeforeChangeCount);
        Assert.AreEqual(1, _module.AfterChangeCount);
        Assert.IsNull(_module.LastOld, "Old config should be null on the first assignment.");
        Assert.AreSame(_config, _module.LastNew);
    }

    [Test]
    public void SettingConfig_ToSameValue_IsIgnored()
    {
        _module.Config = _config;
        int before = _module.BeforeChangeCount;
        int after  = _module.AfterChangeCount;

        _module.Config = _config; // same reference

        Assert.AreEqual(before, _module.BeforeChangeCount, "Re-assigning the same config should not re-fire hooks.");
        Assert.AreEqual(after,  _module.AfterChangeCount);
    }

    [Test]
    public void OnDisable_DisablesModule()
    {
        UnityEngine.Object.DestroyImmediate(_go); // triggers OnDisable
        _go = null;

        Assert.AreEqual(1, _module.DisableCount, "OnDisable should call DisableModule once.");
    }
}
