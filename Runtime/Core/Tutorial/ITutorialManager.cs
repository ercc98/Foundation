using System;

namespace ErccDev.Foundation.Core.Tutorial
{
    public interface ITutorialManager
    {
        bool IsRunning { get; }

        void StartTutorial();
        void NextStep();
        void SkipTutorial();
        event Action OnTutorialEnded;
    }
}
