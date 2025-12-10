using System;
using UnityEngine;
using ErccDev.Foundation.Audio;

namespace ErccDev.Foundation.Core.Gameplay
{
    /// <summary>Base orchestrator for a single gameplay session.</summary>
    public abstract class GameSessionController : MonoBehaviour, IGameSessionLifecycle
    {
        [Header("Optional Services")]
        [Tooltip("Assign a MonoBehaviour that implements IAudioService (e.g., your AudioManagerBase).")]
        [SerializeField] private MonoBehaviour audioProvider;

        protected IAudioService AudioService { get; private set; }

        public bool IsSessionActive { get; private set; }
        public bool IsSessionOver   { get; private set; }

        public event Action SessionStarted;
        public event Action SessionEnded;
        public event Action SessionRestarted;

        protected virtual void Awake() => ResolveAudioService();

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

        protected void ResolveAudioService()
        {
            if (audioProvider is IAudioService svc)
            {
                AudioService = svc;
                return;
            }

            // Fallback: auto-find any AudioManagerBase in scene
            var manager = FindAnyObjectByType<AudioManagerBase>(FindObjectsInactive.Exclude);
            AudioService = manager;
        }
    }
}
