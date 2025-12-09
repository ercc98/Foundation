using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ErccDev.Foundation.Core.Animations;

public class AnimationEventDisableTests
{
    private GameObject _parent;
    private GameObject _child;
    private AnimationEventDisable _disabler;

    [SetUp]
    public void SetUp()
    {
        _parent = new GameObject("Parent");
        _child = new GameObject("Child");
        _child.transform.SetParent(_parent.transform);

        _disabler = _child.AddComponent<AnimationEventDisable>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_parent != null)
            UnityEngine.Object.Destroy(_parent);
    }

    [Test]
    public void AE_DisableSelf_DisablesGameObject_WhenParentActive()
    {
        _parent.SetActive(true);
        _child.SetActive(true);

        _disabler.AE_DisableSelf();

        Assert.IsFalse(_child.activeSelf, "Child should be disabled when AE_DisableSelf is called and parent is active.");
    }

    [Test]
    public void AE_DisableSelf_DoesNotDisable_WhenParentInactive()
    {
        _parent.SetActive(false);
        _child.SetActive(true);

        _disabler.AE_DisableSelf();

        Assert.IsTrue(_child.activeSelf, "Child should remain active if parent is inactive (per implementation).");
    }

    [Test]
    public void AE_DisableSelfDelay_NonPositive_DisablesImmediately()
    {
        _parent.SetActive(true);
        _child.SetActive(true);

        _disabler.AE_DisableSelfDelay(0f);

        Assert.IsFalse(_child.activeSelf, "Child should be disabled immediately when delay <= 0.");
    }

    [UnityTest]
    public IEnumerator AE_DisableSelfDelay_Positive_DisablesAfterDelay()
    {
        _parent.SetActive(true);
        _child.SetActive(true);

        float delay = 0.2f;
        _disabler.AE_DisableSelfDelay(delay);

        // Immediately after call, it should still be active
        Assert.IsTrue(_child.activeSelf, "Child should still be active immediately after AE_DisableSelfDelay is called.");

        // Wait a bit less than the delay
        yield return new WaitForSeconds(delay * 0.5f);
        Assert.IsTrue(_child.activeSelf, "Child should still be active before the delay time.");

        // Wait past the delay
        yield return new WaitForSeconds(delay);
        Assert.IsFalse(_child.activeSelf, "Child should be disabled after the delay has passed.");
    }
}
