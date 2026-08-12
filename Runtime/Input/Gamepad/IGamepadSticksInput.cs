using System;
using UnityEngine;

namespace ErccDev.Foundation.Input.Gamepad
{
    /// <summary>
    /// Analog sticks. Both axes are radially dead-zoned and clamped to the -1..1 range;
    /// mapping the vectors to movement/aim is left to the consumer.
    /// </summary>
    public interface IGamepadSticksInput
    {
        event Action<Vector2> MoveChanged;   // left stick
        event Action<Vector2> LookChanged;   // right stick

        Vector2 Move { get; }   // -1..1
        Vector2 Look { get; }   // -1..1

        bool IsConnected { get; }
    }
}
