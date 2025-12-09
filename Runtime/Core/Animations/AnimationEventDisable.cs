using System.Collections;
using UnityEngine;

namespace ErccDev.Foundation.Core.Animations
{
    [AddComponentMenu("ErccDev/Animations/Animation Event Disable")]
    public sealed class AnimationEventDisable : MonoBehaviour, IAnimationDisabler
    {
        // Animation Event – disable immediately
        public void AE_DisableSelf()
        {
            if (transform.parent != null && !transform.parent.gameObject.activeInHierarchy)
                return;

            DisableSelf();
        }

        // Animation Event – disable after delay
        public void AE_DisableSelfDelay(float seconds)
        {
            DisableSelf(seconds);
        }

        // -------- IAnimationDisabler implementation --------

        public void DisableSelf()
        {
            gameObject.SetActive(false);
        }

        public void DisableSelf(float delaySeconds)
        {
            if (delaySeconds <= 0f)
            {
                DisableSelf();
                return;
            }

            StartCoroutine(DisableAfter(delaySeconds));
        }

        private IEnumerator DisableAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            DisableSelf();
        }
    }
}