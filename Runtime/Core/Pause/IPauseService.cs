using System;

namespace ErccDev.Foundation.Pause
{
    public interface IPauseService
    {
        bool IsPaused { get; }

        event Action<bool, string> Changed;

        void Pause(string reason = "Pause");
        void Resume(string reason = "Resume");
        void Toggle(string reason = "Toggle");
    }
}