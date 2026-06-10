using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Core.Achievements;
using ErccDev.Foundation.Core.Achievements.Conditions;
using ErccDev.Foundation.Core.Events;
using ErccDev.Foundation.Core.Save;

public class AchievementManagerBaseTests
{
    private const string EventName = "test.count";
    private const string AchievementId = "ach1";

    private GameObject _busGo;
    private GameObject _managerGo;
    private AchievementManagerBase _manager;
    private EventCountCondition _condition;
    private AchievementDefinition _def;
    private ISaveService _originalSave;

    // In-memory save so the test never touches disk.
    private class InMemorySave : ISaveService
    {
        private readonly Dictionary<string, object> _store = new();

        public void SaveObject<T>(T data, string fileName, bool pretty = true) => _store[fileName] = data;

        public bool TryLoadObject<T>(string fileName, out T data)
        {
            if (_store.TryGetValue(fileName, out var o) && o is T t) { data = t; return true; }
            data = default;
            return false;
        }

        public void SaveSO<T>(T data, string fileName, bool pretty = true) where T : ScriptableObject { }
        public void LoadSO<T>(T data, string fileName) where T : ScriptableObject { }
        public void SaveAllSO(List<ScriptableObject> dataObjects, string fileName, bool pretty = true) { }
        public void LoadAllSO(List<ScriptableObject> dataObjects, string fileName) { }
    }

    [SetUp]
    public void SetUp()
    {
        _originalSave = SaveService.Default;
        SaveService.SetDefault(new InMemorySave());

        // EventBus singleton the condition listens on.
        _busGo = new GameObject("EventBus_GO");
        _busGo.AddComponent<EventBus>().SendMessage("Awake", SendMessageOptions.DontRequireReceiver);

        _condition = ScriptableObject.CreateInstance<EventCountCondition>();
        _condition.eventName = EventName;
        _condition.target    = 3;

        _def = ScriptableObject.CreateInstance<AchievementDefinition>();
        _def.achievementId = AchievementId;
        _def.condition     = _condition;

        // Build the manager inactive so we can inject serialized fields before Awake runs.
        _managerGo = new GameObject("AchievementManager_Test");
        _managerGo.SetActive(false);
        _manager = _managerGo.AddComponent<AchievementManagerBase>();

        SetPrivate(_manager, "achievements", new List<AchievementDefinition> { _def });
        SetPrivate(_manager, "saveFileName", "achievements_test");

        _managerGo.SetActive(true); // Awake => LoadProgress + condition.Initialize (StartListening)
    }

    [TearDown]
    public void TearDown()
    {
        if (_managerGo != null) UnityEngine.Object.DestroyImmediate(_managerGo); // OnDestroy => condition.Cleanup
        if (_def != null)       UnityEngine.Object.DestroyImmediate(_def);
        if (_condition != null) UnityEngine.Object.DestroyImmediate(_condition);
        if (_busGo != null)     UnityEngine.Object.DestroyImmediate(_busGo);
        ForceClearEventBusSingleton();

        SaveService.SetDefault(_originalSave);
    }

    [Test]
    public void ReachingTarget_UnlocksAchievement_AndRaisesEvent()
    {
        AchievementDefinition unlocked = null;
        _manager.OnUnlocked += d => unlocked = d;

        for (int i = 0; i < 3; i++)
            EventBus.Trigger(EventName);

        _manager.Evaluate();

        Assert.IsTrue(_manager.IsUnlocked(AchievementId), "Reaching the target count should unlock the achievement.");
        Assert.AreSame(_def, unlocked, "OnUnlocked should report the unlocked definition.");
        Assert.AreEqual(1f, _manager.GetProgress01(AchievementId), 0.001f);
    }

    [Test]
    public void BelowTarget_DoesNotUnlock_AndReportsPartialProgress()
    {
        EventBus.Trigger(EventName); // 1 of 3
        _manager.Evaluate();

        Assert.IsFalse(_manager.IsUnlocked(AchievementId), "One event should not satisfy a target of 3.");
        Assert.AreEqual(1f / 3f, _manager.GetProgress01(AchievementId), 0.001f);
    }

    [Test]
    public void AmountPayload_CountsByAmount()
    {
        _manager.OnUnlocked += _ => { };

        // amountKey lets a single event contribute more than +1.
        _condition.amountKey = "amount";
        // re-init so the new amountKey takes effect on a fresh count
        _condition.Cleanup();
        _condition.Initialize(null);

        EventBus.Trigger(EventName, new Dictionary<string, object> { ["amount"] = 3 });
        _manager.Evaluate();

        Assert.IsTrue(_manager.IsUnlocked(AchievementId), "A payload amount that meets the target should unlock.");
    }

    // ---------- helpers ----------

    static void SetPrivate(object target, string field, object value)
    {
        target.GetType()
              .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)
              ?.SetValue(target, value);
    }

    static void ForceClearEventBusSingleton()
    {
        typeof(EventBus)
            .GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)
            ?.SetValue(null, null);
    }
}
