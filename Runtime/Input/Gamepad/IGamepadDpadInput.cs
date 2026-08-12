using System;

namespace ErccDev.Foundation.Input.Gamepad
{
    /// <summary>
    /// Discrete four-direction D-pad. Fires directional events on state change and exposes
    /// the current held direction as booleans (a diagonal holds two at once).
    /// </summary>
    public interface IGamepadDpadInput
    {
        event Action DpadUp;
        event Action DpadDown;
        event Action DpadLeft;
        event Action DpadRight;
        event Action DpadReturnedToCenterX;
        event Action DpadReturnedToCenterY;

        bool IsUp    { get; }
        bool IsDown  { get; }
        bool IsLeft  { get; }
        bool IsRight { get; }
    }
}
