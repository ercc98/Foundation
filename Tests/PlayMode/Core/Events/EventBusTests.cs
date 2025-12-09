using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools; // for UnityTest (frame advance) if needed
using ErccDev.Foundation.Core.Events;

public class EventBusTests
{
    private GameObject _go;
    private EventBus _bus;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("EventBus_GO");
        _bus = _go.AddComponent<EventBus>();
        // Simulate Unity lifecycle
        _bus.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
        Assert.IsNotNull(EventBus.Instance, "Instance should be set in Awake.");
    }

    [TearDown]
    public void TearDown()
    {
        if (_bus) UnityEngine.Object.DestroyImmediate(_bus.gameObject);
        ForceClearSingleton(); // ensure static cleared between tests
    }

    // --- Helpers ---
    private static Dictionary<string, Delegate> PeekMap(EventBus bus)
    {
        var f = typeof(EventBus).GetField("_map", BindingFlags.Instance | BindingFlags.NonPublic);
        var raw = (Dictionary<string, Action<Dictionary<string, object>>>)f.GetValue(bus);
        // copy to generic Dictionary<string,Delegate> for easier key-only checks (or just return raw)
        var copy = new Dictionary<string, Delegate>(StringComparer.Ordinal);
        foreach (var kv in raw) copy[kv.Key] = kv.Value;
        return copy;
    }

    private static void ForceClearSingleton()
    {
        var f = typeof(EventBus).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
        f.SetValue(null, null);
    }

    // ---------- Tests ----------

    [UnityTest]
    public System.Collections.IEnumerator Awake_SetsSingleton_And_DestroysDuplicates()
    {
        // First one is the singleton
        Assert.AreSame(_bus, EventBus.Instance);

        // Create a duplicate; it should destroy itself in Awake
        var otherGO = new GameObject("EventBus_GO_2");
        var other = otherGO.AddComponent<EventBus>();
        other.SendMessage("Awake", SendMessageOptions.DontRequireReceiver);
        yield return null; // allow Destroy to process end-of-frame

        Assert.AreSame(_bus, EventBus.Instance);
        Assert.IsTrue(other == null || other.Equals(null), "Duplicate EventBus should be destroyed.");
    }

    [Test]
    public void StartListening_And_Trigger_Invoke_Listener_With_Payload()
    {
        int called = 0;
        int payloadAmount = 0;

        void Listener(Dictionary<string, object> payload)
        {
            called++;
            if (payload != null && payload.TryGetValue("amount", out var v)) payloadAmount += Convert.ToInt32(v);
        }

        EventBus.StartListening("addCoins", Listener);

        EventBus.Trigger("addCoins", new() { ["amount"] = 2 });
        EventBus.Trigger("addCoins", new() { ["amount"] = 3 });

        Assert.AreEqual(2, called);
        Assert.AreEqual(5, payloadAmount);
    }

    [Test]
    public void StopListening_Removes_Listener_And_Clears_Key_When_Last()
    {
        int called = 0;
        void L(Dictionary<string, object> _) => called++;

        EventBus.StartListening("evt", L);
        EventBus.Trigger("evt");
        Assert.AreEqual(1, called);

        EventBus.StopListening("evt", L);
        EventBus.Trigger("evt");
        Assert.AreEqual(1, called, "Listener should not be called after unsubscribe.");

        var map = PeekMap(EventBus.Instance);
        Assert.IsFalse(map.ContainsKey("evt"), "Event key should be removed after last listener unsubscribes.");
    }

    [Test]
    public void MultipleListeners_AllInvoke_Then_Unsubscribe_One()
    {
        int a = 0, b = 0;
        void LA(Dictionary<string, object> _) => a++;
        void LB(Dictionary<string, object> _) => b++;

        EventBus.StartListening("multi", LA);
        EventBus.StartListening("multi", LB);

        EventBus.Trigger("multi");
        Assert.AreEqual(1, a);
        Assert.AreEqual(1, b);

        EventBus.StopListening("multi", LA);
        EventBus.Trigger("multi");

        Assert.AreEqual(1, a, "A should not be called after unsubscribe.");
        Assert.AreEqual(2, b, "B should still be called.");
    }

    [Test]
    public void Safe_When_Null_Listener_Or_No_Instance()
    {
        // Null listener should be ignored
        Assert.DoesNotThrow(() => EventBus.StartListening("x", null));
        Assert.DoesNotThrow(() => EventBus.StopListening("x", null));

        // Remove current instance and ensure static API doesn't throw
        UnityEngine.Object.DestroyImmediate(_bus.gameObject);
        ForceClearSingleton();
        Assert.IsNull(EventBus.Instance);

        Assert.DoesNotThrow(() => EventBus.Trigger("nope"));
        Assert.DoesNotThrow(() => EventBus.StartListening("nope", _ => { }));
        Assert.DoesNotThrow(() => EventBus.StopListening("nope", _ => { }));
    }

    [Test]
    public void Trigger_With_No_Registered_Listener_Does_Nothing()
    {
        // Nothing registered for 'unknown'; should not throw
        Assert.DoesNotThrow(() => EventBus.Trigger("unknown"));
        // Ensure map has no key created accidentally
        var map = PeekMap(EventBus.Instance);
        Assert.IsFalse(map.ContainsKey("unknown"));
    }
}