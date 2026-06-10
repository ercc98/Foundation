using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using ErccDev.Foundation.Input.Swipe;

public class SwipeInputSystemTests
{
    private GameObject _go;
    private SwipeInputSystem _system;
    private SwipeInputConfig _config;

    [SetUp]
    public void SetUp()
    {
        _go     = new GameObject("SwipeInput_Test");
        _system = _go.AddComponent<SwipeInputSystem>();

        _config = ScriptableObject.CreateInstance<SwipeInputConfig>();
        _config.minSwipePixels = 50f;
        _config.tapMaxPixels   = 20f;
        _config.tapMaxTime     = 0.25f;

        _system.Config = _config;

        // Pin DpiScale so pixel thresholds are deterministic regardless of the
        // test machine's screen DPI (otherwise minSwipe = pixels * DpiScale drifts).
        SetDpiScale(_system, 1f);
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)     UnityEngine.Object.DestroyImmediate(_go);
        if (_config != null) UnityEngine.Object.DestroyImmediate(_config);
    }

    [Test]
    public void SmallQuickMovement_FiresTap()
    {
        bool tapped = false;
        _system.Tap += () => tapped = true;

        Resolve(new Vector2(5f, 5f), 0.1f); // within tapMaxPixels and tapMaxTime

        Assert.IsTrue(tapped, "A small, quick press should register as a tap.");
    }

    [Test]
    public void SmallButSlowMovement_DoesNotTap()
    {
        bool tapped = false;
        _system.Tap += () => tapped = true;

        Resolve(new Vector2(5f, 5f), 1.0f); // held too long to be a tap

        Assert.IsFalse(tapped, "Holding past tapMaxTime should not be a tap.");
    }

    [Test]
    public void RightwardDrag_FiresSwipeRight()
    {
        bool right = false;
        bool left  = false;
        _system.SwipeRight += () => right = true;
        _system.SwipeLeft  += () => left  = true;

        Resolve(new Vector2(100f, 10f), 0.3f);

        Assert.IsTrue(right, "A dominant +X drag should swipe right.");
        Assert.IsFalse(left);
    }

    [Test]
    public void LeftwardDrag_FiresSwipeLeft()
    {
        bool left = false;
        _system.SwipeLeft += () => left = true;

        Resolve(new Vector2(-100f, 10f), 0.3f);

        Assert.IsTrue(left, "A dominant -X drag should swipe left.");
    }

    [Test]
    public void UpwardDrag_FiresSwipeUp()
    {
        bool up = false;
        _system.SwipeUp += () => up = true;

        Resolve(new Vector2(10f, 100f), 0.3f);

        Assert.IsTrue(up, "A dominant +Y drag should swipe up.");
    }

    [Test]
    public void DownwardDrag_FiresSwipeDown()
    {
        bool down = false;
        _system.SwipeDown += () => down = true;

        Resolve(new Vector2(10f, -100f), 0.3f);

        Assert.IsTrue(down, "A dominant -Y drag should swipe down.");
    }

    [Test]
    public void MidRangeMovement_NeitherTapNorSwipe()
    {
        bool fired = false;
        _system.Tap        += () => fired = true;
        _system.SwipeLeft  += () => fired = true;
        _system.SwipeRight += () => fired = true;
        _system.SwipeUp    += () => fired = true;
        _system.SwipeDown  += () => fired = true;

        // 30px: above tapMaxPixels (20) but below minSwipePixels (50) -> ignored
        Resolve(new Vector2(30f, 0f), 0.3f);

        Assert.IsFalse(fired, "Movement between the tap and swipe thresholds should be ignored.");
    }

    // ---------- helpers ----------

    void Resolve(Vector2 delta, double heldFor) =>
        InvokePrivate(_system, "ResolveGesture", delta, heldFor);

    // DpiScale is a protected auto-property on the InputModule<T> base; set its
    // compiler-generated backing field, walking up to the declaring base type.
    static void SetDpiScale(object target, float value)
    {
        const string backing = "<DpiScale>k__BackingField";
        for (var t = target.GetType(); t != null; t = t.BaseType)
        {
            var field = t.GetField(backing, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                return;
            }
        }
    }

    static void InvokePrivate(object target, string method, params object[] args)
    {
        target.GetType()
              .GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)
              ?.Invoke(target, args);
    }
}
