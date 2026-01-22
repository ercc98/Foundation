using System;

namespace ErccDev.Foundation.Input
{
    public interface ITouchInput
    {
        event Action StartTap;
        event Action EndTap;
        event Action Tap;
    }
}