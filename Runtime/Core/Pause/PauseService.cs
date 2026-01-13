using System;

namespace ErccDev.Foundation.Pause
{
    public sealed class PauseService : IPauseService
    {
        public bool IsPaused { get; private set; }

        public event Action<bool, string> Changed;

        public void Pause(string reason = "Pause")
        {
            if (IsPaused) return;
            IsPaused = true;
            Changed?.Invoke(true, reason);
        }

        public void Resume(string reason = "Resume")
        {
            if (!IsPaused) return;
            IsPaused = false;
            Changed?.Invoke(false, reason);
        }

        public void Toggle(string reason = "Toggle")
        {
            if (IsPaused) Resume(reason);
            else Pause(reason);
        }
    }
}