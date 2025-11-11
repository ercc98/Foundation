using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ErccDev.Core.Animations
{
    /// <summary>
    /// Generic notifier for "animation ended".
    /// Use an Animation Event on your clip to call AE_End() (or AE_EndDelay(float)).
    /// Subscribed listeners (code/Inspector) will be notified; optional auto-disable.
    /// </summary>
    [AddComponentMenu("ErccDev/Animations/Animation End Notifier")]
    public sealed class AnimationEndNotifier : MonoBehaviour
    {
        [Tooltip("Disable this GameObject after notifying listeners.")]
        [SerializeField] private bool autoDisableOnEnd = false;

        /// <summary>Typed event for code subscribers.</summary>
        public event Action Ended;

        /// <summary>Inspector-friendly UnityEvent.</summary>
        [SerializeField] private UnityEvent onEnded;

        // -------- Animation Events (call these from your clip) --------

        /// <summary>Animation Event: no-arg end.</summary>
        public void AE_End()
        {
            Notify();
        }

        /// <summary>Animation Event: end after a small delay (use AnimationEvent 'float' param).</summary>
        public void AE_EndDelay(float seconds)
        {
            if (seconds <= 0f) { Notify(); return; }
            StartCoroutine(EndAfter(seconds));
        }

        /// <summary>Manual trigger from code if needed.</summary>
        public void TriggerEnd() => Notify();

        // -------- Internals --------

        IEnumerator EndAfter(float s)
        {
            yield return new WaitForSeconds(s);
            Notify();
        }

        void Notify()
        {
            try { Ended?.Invoke(); } catch (Exception e) { Debug.LogException(e, this); }
            try { onEnded?.Invoke(); } catch (Exception e) { Debug.LogException(e, this); }

            if (autoDisableOnEnd)
                gameObject.SetActive(false);
        }
    }
}