using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Core.Events;
using ErccDev.Foundation.Core.Notifications;

public class NotificationManagerBaseTests
{
    // Test seam: skips DontDestroyOnLoad so plain GameObjects can be created and torn down.
    private class TestableNotificationManager : NotificationManagerBase
    {
        protected override void Awake() { /* no persistence in tests */ }
    }

    private GameObject _go;
    private GameObject _busGo;

    [SetUp]
    public void SetUp()
    {
        // EventBus is a DontDestroyOnLoad singleton; give the tests one to publish into.
        _busGo = new GameObject("eventbus");
        _busGo.AddComponent<EventBus>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go    != null) Object.DestroyImmediate(_go);
        if (_busGo != null) Object.DestroyImmediate(_busGo);
    }

    private TestableNotificationManager Build()
    {
        _go = new GameObject("notifications");
        return _go.AddComponent<TestableNotificationManager>();   // OnEnable registers as default
    }

    // Duration 0 drains the queue synchronously, so order can be asserted without frame waits.
    private static NotificationData Toast(string title)
        => new NotificationData(title, duration: 0f);

    [Test]
    public void Notify_Raises_OnNotification_With_The_Data()
    {
        var mgr = Build();
        NotificationData got = default;
        int fired = 0;
        mgr.OnNotification += d => { got = d; fired++; };

        mgr.Notify(Toast("Hello"));

        Assert.AreEqual(1, fired);
        Assert.AreEqual("Hello", got.Title);
    }

    [Test]
    public void Notifications_Surface_In_FIFO_Order()
    {
        var mgr = Build();
        var seen = new List<string>();
        mgr.OnNotification += d => seen.Add(d.Title);

        mgr.Notify(Toast("a"));
        mgr.Notify(Toast("b"));
        mgr.Notify(Toast("c"));

        Assert.AreEqual(new[] { "a", "b", "c" }, seen.ToArray());
    }

    [Test]
    public void Manager_Registers_As_NotificationService_Default()
    {
        var mgr = Build();
        Assert.AreSame(mgr, NotificationService.Default);

        // Pushing through the static facade reaches the manager.
        int fired = 0;
        mgr.OnNotification += _ => fired++;
        NotificationService.Notify("via facade", duration: 0f);

        Assert.AreEqual(1, fired);
    }

    [Test]
    public void Notify_Triggers_notificationShown_EventBus_Event()
    {
        var mgr = Build();
        string title = null;
        void Listener(Dictionary<string, object> p) => title = p["title"] as string;
        EventBus.StartListening("notificationShown", Listener);

        mgr.Notify(Toast("Boom"));

        EventBus.StopListening("notificationShown", Listener);
        Assert.AreEqual("Boom", title);
    }
}
