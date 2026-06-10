using NUnit.Framework;
using ErccDev.Foundation.Pause;

public class PauseServiceTests
{
    private PauseService _service;

    [SetUp]
    public void SetUp() => _service = new PauseService();

    [Test]
    public void NewService_StartsUnpaused()
    {
        Assert.IsFalse(_service.IsPaused, "A fresh service should not be paused.");
    }

    [Test]
    public void Pause_SetsPaused_AndRaisesChanged()
    {
        bool   raised = false;
        bool   state  = false;
        string reason = null;
        _service.Changed += (s, r) => { raised = true; state = s; reason = r; };

        _service.Pause("menu");

        Assert.IsTrue(_service.IsPaused);
        Assert.IsTrue(raised,  "Changed should fire when pausing.");
        Assert.IsTrue(state,   "Changed should report the paused state as true.");
        Assert.AreEqual("menu", reason);
    }

    [Test]
    public void Pause_WhenAlreadyPaused_DoesNotRaiseAgain()
    {
        _service.Pause();

        int count = 0;
        _service.Changed += (_, _) => count++;
        _service.Pause();

        Assert.AreEqual(0, count, "Pausing while already paused should be a no-op.");
    }

    [Test]
    public void Resume_ClearsPaused_AndRaisesChanged()
    {
        _service.Pause();

        bool state = true;
        _service.Changed += (s, _) => state = s;
        _service.Resume("resume");

        Assert.IsFalse(_service.IsPaused);
        Assert.IsFalse(state, "Changed should report the paused state as false on resume.");
    }

    [Test]
    public void Resume_WhenNotPaused_DoesNotRaise()
    {
        int count = 0;
        _service.Changed += (_, _) => count++;
        _service.Resume();

        Assert.AreEqual(0, count, "Resuming while not paused should be a no-op.");
    }

    [Test]
    public void Toggle_FlipsBetweenStates()
    {
        _service.Toggle();
        Assert.IsTrue(_service.IsPaused, "First toggle should pause.");

        _service.Toggle();
        Assert.IsFalse(_service.IsPaused, "Second toggle should resume.");
    }
}
