using System;
using UnityEngine;

namespace ErccDev.Foundation.Input
{
    public interface ITouchInput
    {
        event Action StartTouch;
        event Action EndTouch;

        bool IsTouching { get; }
        Vector2 PointerPosition { get; }
        Vector2 StartPosition { get; }
    }
}
