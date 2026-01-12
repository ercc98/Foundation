using UnityEngine;

namespace ErccDev.Foundation.Core.Tutorial
{
    [CreateAssetMenu(menuName = "ErccDev/Tutorial/Tutorial Step")]
    public class TutorialStep : ScriptableObject
    {
        [Header("Meta")]
        public string stepId;
        public bool skippable = true;

        [Header("UI")]
        [TextArea]
        public string instructionText;
        public string arrowAnimationName;

        [Header("Behavior")]
        public TutorialCondition condition;

        public void Initialize(ITutorialContext context)
        {
            condition?.Initialize(context);
        }

        public bool IsCompleted()
        {
            return condition != null && condition.IsCompleted();
        }

        public void Cleanup()
        {
            condition?.Cleanup();
        }
    }
}