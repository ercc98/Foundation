using System;

namespace ErccDev.Foundation.Input.Touch
{
    public interface IVerticalTouchInput
    {
        event Action MovedUp;
        event Action MovedDown;
        event Action ReturnedToCenterY;

        float NormalizedY { get; }   // 0..1
        float SteeringY { get; }     // -1..1

        bool IsTopSide { get; }
        bool IsBottomSide { get; }
        bool IsCenterY { get; }
    }
}
