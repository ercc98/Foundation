using System;

namespace ErccDev.Foundation.Input.Gamepad
{
    /// <summary>
    /// Digital face and shoulder buttons, reported by controller-agnostic position.
    /// </summary>
    public interface IGamepadButtonsInput
    {
        event Action<GamepadButton> ButtonPressed;
        event Action<GamepadButton> ButtonReleased;

        bool IsPressed(GamepadButton button);
    }
}
