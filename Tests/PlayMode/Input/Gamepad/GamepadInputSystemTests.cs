using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Input.Gamepad;

public class GamepadInputSystemTests
{
    private GameObject _go;
    private GamepadInputSystem _system;
    private GamepadInputConfig _config;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("GamepadInput_Test");
        _system = _go.AddComponent<GamepadInputSystem>();

        _config = ScriptableObject.CreateInstance<GamepadInputConfig>();
        _config.stickDeadZone    = 0.15f;
        _config.triggerThreshold = 0.1f;
        _config.dpadThreshold    = 0.5f;

        _system.Config = _config;
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            UnityEngine.Object.Destroy(_go);
        if (_config != null)
            UnityEngine.Object.Destroy(_config);
    }

    // ---------- Sticks ----------

    [Test]
    public void Move_InsideDeadZone_ReadsZero()
    {
        SetRawMove(new Vector2(0.1f, 0.05f));

        Assert.AreEqual(Vector2.zero, _system.Move, "Stick input inside the dead zone should read as zero.");
    }

    [Test]
    public void Move_AtFullTilt_ReadsUnitMagnitude()
    {
        SetRawMove(Vector2.right);

        Assert.Greater(_system.Move.x, 0f, "Full right tilt should produce positive X.");
        Assert.AreEqual(1f, _system.Move.magnitude, 1e-4f, "Full tilt should map to unit magnitude.");
    }

    [Test]
    public void Move_PastDeadZone_RescalesFromZero()
    {
        // Just past the 0.15 dead zone should be near 0, not near 0.15.
        SetRawMove(new Vector2(0.16f, 0f));

        Assert.Less(_system.Move.magnitude, 0.05f, "Output should ramp from 0 immediately past the dead zone.");
    }

    // ---------- Triggers ----------

    [Test]
    public void Trigger_BelowThreshold_ReadsZeroAndNotHeld()
    {
        SetPrivate(_system, "_rawLeftTrigger", 0.05f);

        Assert.AreEqual(0f, _system.LeftTrigger, "Trigger below threshold should read 0.");
        Assert.IsFalse(_system.IsLeftTriggerHeld);
    }

    [Test]
    public void Trigger_AboveThreshold_IsHeld()
    {
        SetPrivate(_system, "_rawRightTrigger", 0.6f);

        Assert.AreEqual(0.6f, _system.RightTrigger, 1e-4f);
        Assert.IsTrue(_system.IsRightTriggerHeld);
    }

    // ---------- D-pad ----------

    [Test]
    public void Dpad_Left_And_Up_Diagonal_HoldsBoth()
    {
        SetPrivate(_system, "_rawDpad", new Vector2(-1f, 1f));

        Assert.IsTrue(_system.IsLeft);
        Assert.IsTrue(_system.IsUp);
        Assert.IsFalse(_system.IsRight);
        Assert.IsFalse(_system.IsDown);
    }

    [Test]
    public void Dpad_BelowThreshold_IsCentered()
    {
        SetPrivate(_system, "_rawDpad", new Vector2(0.3f, -0.3f));

        Assert.IsFalse(_system.IsLeft);
        Assert.IsFalse(_system.IsRight);
        Assert.IsFalse(_system.IsUp);
        Assert.IsFalse(_system.IsDown);
    }

    // ---------- Buttons ----------

    [Test]
    public void Button_PressAndRelease_TracksStateAndRaisesEvents()
    {
        GamepadButton? pressed = null;
        GamepadButton? released = null;

        _system.ButtonPressed  += b => pressed = b;
        _system.ButtonReleased += b => released = b;

        InvokeSetButton(GamepadButton.South, true);

        Assert.IsTrue(_system.IsPressed(GamepadButton.South));
        Assert.AreEqual(GamepadButton.South, pressed);

        InvokeSetButton(GamepadButton.South, false);

        Assert.IsFalse(_system.IsPressed(GamepadButton.South));
        Assert.AreEqual(GamepadButton.South, released);
    }

    [Test]
    public void Button_RepeatedPress_RaisesEventOnce()
    {
        int count = 0;
        _system.ButtonPressed += _ => count++;

        InvokeSetButton(GamepadButton.East, true);
        InvokeSetButton(GamepadButton.East, true);

        Assert.AreEqual(1, count, "Holding a button should not re-raise ButtonPressed.");
    }

    // ---------- helpers ----------

    void SetRawMove(Vector2 value) => SetPrivate(_system, "_rawMove", value);

    void InvokeSetButton(GamepadButton button, bool pressed)
    {
        _system.GetType()
               .GetMethod("SetButton", BindingFlags.Instance | BindingFlags.NonPublic)
               ?.Invoke(_system, new object[] { button, pressed });
    }

    static void SetPrivate(object target, string field, object value)
    {
        target.GetType()
              .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)
              ?.SetValue(target, value);
    }
}
