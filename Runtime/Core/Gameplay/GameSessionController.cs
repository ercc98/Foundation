using System;
using UnityEngine;
using ErccDev.Foundation.Audio;

namespace ErccDev.Foundation.Core.Gameplay
{
    /// <summary>Base orchestrator for a single gameplay session.</summary>
    public abstract class GameSessionController : MonoBehaviour, IGameSessionLifecycle
    {

        public bool IsSessionActive { get; private set; }
        public bool IsSessionOver   { get; private set; }

        public event Action SessionStarted;
        public event Action SessionEnded;
        public event Action SessionRestarted;
        

        public void StartSession()
        {
            if (IsSessionActive) return;
            ResetSessionState();
            IsSessionActive = true;
            IsSessionOver = false;
            SessionStarted?.Invoke();
            OnSessionStarted();
        }

        public void EndSession()
        {
            if (!IsSessionActive || IsSessionOver) return;
            IsSessionActive = false;
            IsSessionOver = true;
            SessionEnded?.Invoke();
            OnSessionEnded();
        }

        public void RestartSession()
        {
            EndSession();
            ResetSessionState();
            IsSessionActive = true;
            IsSessionOver = false;
            SessionRestarted?.Invoke();
            OnSessionRestarted();
        }

        protected abstract void ResetSessionState();

        protected virtual void OnSessionStarted()   { }
        protected virtual void OnSessionEnded()     { }
        protected virtual void OnSessionRestarted() { }

    }
}
