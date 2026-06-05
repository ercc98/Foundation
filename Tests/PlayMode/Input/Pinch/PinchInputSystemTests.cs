using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Input.Pinch;

public class PinchInputSystemTests
{
    private GameObject _go;
    private PinchInputSystem _system;
    private PinchInputConfig _config;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("PinchInput_Test");
        _system = _go.AddComponent<PinchInputSystem>();

        _config = ScriptableObject.CreateInstance<PinchInputConfig>();
        _config.pinchThresholdPixels = 5f;
        _config.lockToFirstTwoFingers = true;

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
    public void FirstStep_BeginsPinch_ScaleIsOne()
    {
        bool started = false;
        _system.PinchStarted += () => started = true;

        Evaluate(0f, 0f, 100f, 0f);

        Assert.IsTrue(_system.IsPinching, "Pinch should begin on the first two-finger step.");
        Assert.IsTrue(started, "PinchStarted should fire when the pinch begins.");
        Assert.AreEqual(100f, _system.StartDistance, 0.001f);
        Assert.AreEqual(1f, _system.Scale, 0.001f, "Scale should be 1 right after the pinch begins.");
    }

    [Test]
    public void FingersMoveApart_FiresPinchedOut_ScaleGreaterThanOne()
    {
        bool pinchedOut = false;
        _system.PinchedOut += () => pinchedOut = true;

        Evaluate(0f, 0f, 100f, 0f);   // begin at distance 100
        Evaluate(0f, 0f, 150f, 0f);   // apart by 50 (> threshold)

        Assert.IsTrue(pinchedOut, "Moving fingers apart past the threshold should fire PinchedOut.");
        Assert.Greater(_system.DeltaPixels, 0f, "DeltaPixels should be positive when fingers move apart.");
        Assert.Greater(_system.Scale, 1f, "Scale should exceed 1 when fingers move apart.");
    }

    [Test]
    public void FingersMoveCloser_FiresPinchedIn_ScaleLessThanOne()
    {
        bool pinchedIn = false;
        _system.PinchedIn += () => pinchedIn = true;

        Evaluate(0f, 0f, 100f, 0f);   // begin at distance 100
        Evaluate(0f, 0f, 40f, 0f);    // closer by 60 (> threshold)

        Assert.IsTrue(pinchedIn, "Moving fingers closer past the threshold should fire PinchedIn.");
        Assert.Less(_system.DeltaPixels, 0f, "DeltaPixels should be negative when fingers move closer.");
        Assert.Less(_system.Scale, 1f, "Scale should be below 1 when fingers move closer.");
    }

    [Test]
    public void MovementBelowThreshold_DoesNotFire()
    {
        bool fired = false;
        _system.PinchedIn  += () => fired = true;
        _system.PinchedOut += () => fired = true;

        Evaluate(0f, 0f, 100f, 0f);   // begin at distance 100
        Evaluate(0f, 0f, 103f, 0f);   // apart by only 3 (< threshold of 5)

        Assert.IsFalse(fired, "Sub-threshold movement should be ignored as jitter.");
    }

    [Test]
    public void Midpoint_IsBetweenTheTwoFingers()
    {
        Evaluate(0f, 0f, 100f, 50f);

        Assert.AreEqual(new Vector2(50f, 25f), _system.Midpoint);
    }

    // ---------- helpers ----------

    void Evaluate(float ax, float ay, float bx, float by) =>
        InvokePrivate(_system, "EvaluatePinch", new Vector2(ax, ay), new Vector2(bx, by));

    static void InvokePrivate(object target, string method, params object[] args)
    {
        target.GetType()
              .GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)
              ?.Invoke(target, args);
    }
}
