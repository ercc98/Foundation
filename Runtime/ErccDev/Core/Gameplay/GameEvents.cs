using System;

namespace ErccDev.Core.Gameplay
{
    /// <summary>Typed global events for simple game flow.</summary>
    public static class GameEvents
    {
        public static event Action GameStarted;
        public static event Action GameOver;

        public static void RaiseGameStarted() => GameStarted?.Invoke();
        public static void RaiseGameOver()    => GameOver?.Invoke();
    }
}