using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Input.Touch;

public class SteeringTouchInputSystemTests
{
    private GameObject _go;
    private SteeringTouchInputSystem _system;
    private SteeringTouchInputConfig _config;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("SteeringTouchInput_Test");
        _system = _go.AddComponent<SteeringTouchInputSystem>();

        _config = ScriptableObject.CreateInstance<SteeringTouchInputConfig>();
        _config.useAbsoluteScreenPosition = true;
        _config.centerDeadZoneNormalizedX = 0.05f;
        _config.centerDeadZoneNormalizedY = 0.05f;

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

    [Test]
    public void NotPressing_SteeringIsZeroOnBothAxes()
    {
        Press(false);
        SetPointer(0f, 0f);

        Assert.AreEqual(0f, _system.SteeringX, "SteeringX should be 0 when not pressing.");
        Assert.AreEqual(0f, _system.SteeringY, "SteeringY should be 0 when not pressing.");
    }

    [Test]
    public void AbsoluteMode_PointerLeftEdge_SteersLeft()
    {
        Press(true);
        SetPointer(0f, Screen.height * 0.5f);

        Assert.Less(_system.SteeringX, 0f, "SteeringX should be negative at the left edge.");
        Assert.IsTrue(_system.IsLeftSide);
        Assert.IsFalse(_system.IsRightSide);
    }

    [Test]
    public void AbsoluteMode_PointerRightEdge_SteersRight()
    {
        Press(true);
        SetPointer(Screen.width, Screen.height * 0.5f);

        Assert.Greater(_system.SteeringX, 0f, "SteeringX should be positive at the right edge.");
        Assert.IsTrue(_system.IsRightSide);
        Assert.IsFalse(_system.IsLeftSide);
    }

    [Test]
    public void AbsoluteMode_PointerTopEdge_SteersUp()
    {
        Press(true);
        SetPointer(Screen.width * 0.5f, Screen.height);

        Assert.Greater(_system.SteeringY, 0f, "SteeringY should be positive at the top edge.");
        Assert.IsTrue(_system.IsTopSide);
        Assert.IsFalse(_system.IsBottomSide);
    }

    [Test]
    public void AbsoluteMode_PointerBottomEdge_SteersDown()
    {
        Press(true);
        SetPointer(Screen.width * 0.5f, 0f);

        Assert.Less(_system.SteeringY, 0f, "SteeringY should be negative at the bottom edge.");
        Assert.IsTrue(_system.IsBottomSide);
        Assert.IsFalse(_system.IsTopSide);
    }

    [Test]
    public void AbsoluteMode_PointerCenter_IsCenterOnBothAxes()
    {
        Press(true);
        SetPointer(Screen.width * 0.5f, Screen.height * 0.5f);

        Assert.IsTrue(_system.IsCenterX, "Center of screen should read as horizontal center (dead zone).");
        Assert.IsTrue(_system.IsCenterY, "Center of screen should read as vertical center (dead zone).");
    }

    // ---------- helpers ----------

    void Press(bool pressing) => SetPrivate(_system, "_isPressing", pressing);

    void SetPointer(float x, float y) =>
        SetPrivate(_system, "_pointerPosition", new Vector2(x, y));

    static void SetPrivate(object target, string field, object value)
    {
        target.GetType()
              .GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)
              ?.SetValue(target, value);
    }
}
