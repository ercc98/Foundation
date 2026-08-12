using System;

namespace ErccDev.Foundation.Input.Gamepad
{
    /// <summary>
    /// Analog shoulder triggers. Values are 0..1; anything below the configured threshold
    /// reads as 0 (and is not considered "held").
    /// </summary>
    public interface IGamepadTriggersInput
    {
        event Action<float> LeftTriggerChanged;
        event Action<float> RightTriggerChanged;

        float LeftTrigger  { get; }   // 0..1
        float RightTrigger { get; }   // 0..1

        bool IsLeftTriggerHeld  { get; }
        bool IsRightTriggerHeld { get; }
    }
}
