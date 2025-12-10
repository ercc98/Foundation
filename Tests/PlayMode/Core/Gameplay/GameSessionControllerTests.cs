using ErccDev.Foundation.Core.Gameplay;
using NUnit.Framework;

sealed class TestSessionControllerTest : GameSessionController
{
    public int resets;
    public int started;
    public int ended;
    public int restarted;

    protected override void ResetSessionState() => resets++;

    protected override void OnSessionStarted()   => started++;
    protected override void OnSessionEnded()     => ended++;
    protected override void OnSessionRestarted() => restarted++;
}

public class GameSessionControllerTests
{
    [Test]
    public void StartSession_SetsFlags_ResetsAndRaisesEventsOnce()
    {
        var c = new TestSessionControllerTest();
        int evStarted = 0;
        c.SessionStarted += () => evStarted++;

        c.StartSession();
        c.StartSession(); // should no-op when already active

        Assert.IsTrue(c.IsSessionActive);
        Assert.IsFalse(c.IsSessionOver);
        Assert.AreEqual(1, c.resets);
        Assert.AreEqual(1, c.started);
        Assert.AreEqual(1, evStarted);
    }

    [Test]
    public void EndSession_SetsFlags_RaisesEvent_OnlyWhenActive()
    {
        var c = new TestSessionControllerTest();
        int evEnded = 0;
        c.SessionEnded += () => evEnded++;

        c.EndSession(); // no-op before start

        c.StartSession();
        c.EndSession();
        c.EndSession(); // no-op when already over

        Assert.IsFalse(c.IsSessionActive);
        Assert.IsTrue(c.IsSessionOver);
        Assert.AreEqual(1, c.ended);
        Assert.AreEqual(1, evEnded);
    }

    [Test]
    public void RestartSession_EndsAndRestarts()
    {
        var c = new TestSessionControllerTest();
        int evRestarted = 0;
        c.SessionRestarted += () => evRestarted++;

        c.StartSession();
        c.RestartSession();

        Assert.IsTrue(c.IsSessionActive);
        Assert.IsFalse(c.IsSessionOver);
        Assert.AreEqual(2, c.resets);      // start + restart
        Assert.AreEqual(1, c.ended);       // from RestartSession
        Assert.AreEqual(1, c.restarted);
        Assert.AreEqual(1, evRestarted);
    }
}
