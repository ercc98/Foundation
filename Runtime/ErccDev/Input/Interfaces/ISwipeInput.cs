using System;

namespace ErccDev.Input
{
    public interface ISwipeInput
    {
        event Action SwipeLeft;
        event Action SwipeRight;
        event Action SwipeUp;
        event Action SwipeDown;
        event Action Tap;
    }
}