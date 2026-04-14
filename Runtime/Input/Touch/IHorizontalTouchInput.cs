using System;
using UnityEngine;

namespace ErccDev.Foundation.Input.Touch
{
    public interface IHorizontalTouchInput
    {
        event Action TouchStarted;
        event Action TouchEnded;
        event Action MovedLeft;
        event Action MovedRight;
        event Action ReturnedToCenter;

        bool IsTouching { get; }
        Vector2 PointerPosition { get; }
        Vector2 StartPosition { get; }

        float NormalizedX { get; }   // 0..1
        float SteeringX { get; }     // -1..1

        bool IsLeftSide { get; }
        bool IsRightSide { get; }
        bool IsCenter { get; }
    }
}