using System;

namespace ErccDev.Foundation.Input.Touch
{
    public interface IHorizontalTouchInput
    {
        event Action MovedLeft;
        event Action MovedRight;
        event Action ReturnedToCenterX;

        float NormalizedX { get; }   // 0..1
        float SteeringX { get; }     // -1..1

        bool IsLeftSide { get; }
        bool IsRightSide { get; }
        bool IsCenterX { get; }
    }
}
