using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ErccDev.Foundation.Core.Animations;

public class AnimationEndNotifierTests
{
    private GameObject _go;
    private AnimationEndNotifier _notifier;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("AnimationEndNotifier_Test");
        _notifier = _go.AddComponent<AnimationEndNotifier>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            UnityEngine.Object.Destroy(_go);
    }

    [Test]
    public void TriggerEnd_RaisesEndedEvent()
    {
        // Arrange
        int callCount = 0;
        _notifier.Ended += () => callCount++;

        // Act
        _notifier.TriggerEnd();

        // Assert
        Assert.AreEqual(1, callCount, "Ended event should be raised exactly once when TriggerEnd is called.");
    }

    [Test]
    public void AE_End_RaisesEndedEvent()
    {
        int callCount = 0;
        _notifier.Ended += () => callCount++;

        _notifier.AE_End();

        Assert.AreEqual(1, callCount, "Ended event should be raised when AE_End is called.");
    }

    [Test]
    public void AE_EndDelay_NonPositive_DoesNotWait()
    {
        int callCount = 0;
        _notifier.Ended += () => callCount++;

        _notifier.AE_EndDelay(0f);

        Assert.AreEqual(1, callCount, "Ended event should be raised immediately when delay <= 0.");
    }

    [UnityTest]
    public IEnumerator AE_EndDelay_WaitsBeforeInvoking()
    {
        int callCount = 0;
        _notifier.Ended += () => callCount++;

        // Act
        _notifier.AE_EndDelay(0.2f);

        // Immediately after call, event shouldn't have fired yet
        Assert.AreEqual(0, callCount, "Ended should not fire immediately when delay > 0.");

        // Wait a bit less than delay
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(0, callCount, "Ended should still not have fired before the delay.");

        // Wait past the delay
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(1, callCount, "Ended should fire once after the delay has passed.");
    }

    [UnityTest]
    public IEnumerator AutoDisableOnEnd_DisablesGameObject()
    {
        // We can't access private field directly, so we use SerializedObject hack
        // or (simpler) we assume you temporarily make it internal or public for tests.
        //
        // For now, we'll assume you changed:
        // [SerializeField] private bool autoDisableOnEnd = false;
        //    → to:
        // [SerializeField] public bool autoDisableOnEnd = false;
        //
        // If you don't want that in production, you can wrap this test in #if UNITY_EDITOR or use reflection.

        _notifier.GetType()
                    .GetField("autoDisableOnEnd", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.SetValue(_notifier, true);

        Assert.IsTrue(_go.activeSelf, "GameObject should start active.");

        _notifier.TriggerEnd();

        // Deactivation happens immediately in Notify().
        yield return null;

        Assert.IsFalse(_go.activeSelf, "GameObject should be deactivated when autoDisableOnEnd is true and end is triggered.");
    }
}
