using System;

namespace ErccDev.Foundation.Core.Animations
{
    /// <summary>
    /// Abstraction for notifying when an animation or sequence finishes.
    /// Implementations raise the Ended event and allow manual triggering.
    /// </summary>
    public interface IAnimationEndNotifier
    {
        /// <summary>
        /// Fired when the animation ends.
        /// </summary>
        event Action Ended;

        /// <summary>
        /// Allows manually triggering the end event.
        /// Useful for testing or non-Animator-based systems.
        /// </summary>
        void TriggerEnd();
    }
}