namespace ErccDev.Foundation.Core.Animations
{
    /// <summary>
    /// Abstraction for disabling a GameObject immediately or after a delay.
    /// Implemented by components that manage object lifetime.
    /// </summary>
    public interface IAnimationDisabler
    {
        /// <summary>
        /// Disable this object immediately.
        /// </summary>
        void DisableSelf();

        /// <summary>
        /// Disable this object after a delay (in seconds).
        /// </summary>
        void DisableSelf(float delaySeconds);
    }
}