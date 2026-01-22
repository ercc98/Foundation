using System;

namespace ErccDev.Foundation.Input
{
    public interface ITouchInput
    {
        event Action StartTouch;
        event Action EndTouch;
        bool IsTouching { get; }
    }
}