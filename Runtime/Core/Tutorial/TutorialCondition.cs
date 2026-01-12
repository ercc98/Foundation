using UnityEngine;

namespace ErccDev.Foundation.Core.Tutorial
{
    public abstract class TutorialCondition : ScriptableObject, ITutorialCondition
    {
        public abstract void Initialize(ITutorialContext context);
        public abstract bool IsCompleted();
        public abstract void Cleanup();

    }
}