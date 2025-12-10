using System;

namespace ErccDev.Foundation.Core.Gameplay
{
    public interface IGameSessionLifecycle
    {
        bool IsSessionActive { get; }
        bool IsSessionOver { get; }

        event Action SessionStarted;
        event Action SessionEnded;
        event Action SessionRestarted;

        void StartSession();
        void EndSession();
        void RestartSession();
    }
}
