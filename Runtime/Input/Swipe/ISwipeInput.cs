using System;

namespace ErccDev.Foundation.Input.Swipe
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