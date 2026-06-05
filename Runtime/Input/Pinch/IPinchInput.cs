using System;
using UnityEngine;

namespace ErccDev.Foundation.Input.Pinch
{
    /// <summary>
    /// Two-finger pinch recognizer. Reports whether the two fingers are moving closer
    /// (<see cref="PinchedIn"/>) or farther apart (<see cref="PinchedOut"/>). Mapping the
    /// gesture to a zoom (camera FOV, orthographic size, UI scale...) is left to the consumer
    /// via <see cref="Scale"/> and <see cref="DeltaPixels"/>.
    /// </summary>
    public interface IPinchInput
    {
        event Action PinchStarted;
        event Action PinchEnded;
        event Action PinchedIn;    // fingers moving closer together
        event Action PinchedOut;   // fingers moving apart

        bool IsPinching { get; }

        float Distance      { get; }   // current distance between the two fingers (pixels)
        float StartDistance { get; }   // distance when the pinch began (pixels)
        float DeltaPixels   { get; }   // signed change since last step (+ apart, - closer), DPI-scaled
        float Scale         { get; }   // current / start distance (1 = unchanged, >1 apart, <1 closer)
        Vector2 Midpoint    { get; }   // screen-space point between the two fingers
    }
}
