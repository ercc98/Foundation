using System.Collections;
using UnityEngine;

namespace ErccDev.Core.Animations
{
    /// <summary>
    /// Call from an Animation Event to disable this GameObject (great for pooled FX/coins).
    /// Methods:
    ///   AE_DisableSelf()                 // disables immediately
    ///   AE_DisableSelfDelay(float secs)  // disables after 'secs'
    /// </summary>
    [AddComponentMenu("ErccDev/Animations/Animation Event Disable")]
    public sealed class AnimationEventDisable : MonoBehaviour
    {
        // No-arg: perfect for AnimationEvent with no parameters
        public void AE_DisableSelf()
        {
            if(!transform.parent.gameObject.activeInHierarchy) return;
            gameObject.SetActive(false);
        }

        // Float arg: use the AnimationEvent 'Float' field to pass a delay
        public void AE_DisableSelfDelay(float seconds)
        {
            if (seconds <= 0f) { gameObject.SetActive(false); return; }
            StartCoroutine(DisableAfter(seconds));
        }

        private IEnumerator DisableAfter(float s)
        {
            yield return new WaitForSeconds(s);
            gameObject.SetActive(false);
        }
    }
}