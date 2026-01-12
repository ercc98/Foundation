using UnityEngine;

namespace ErccDev.Foundation.Core.Tutorial
{
    [RequireComponent(typeof(Collider))]
    public class TutorialTriggerStepBase : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] protected MonoBehaviour tutorialManagerProvider;
        [SerializeField] protected bool triggerOnce = true;
        [SerializeField] protected string requiredTag = "Player";

        protected ITutorialManager tutorialManager;
        protected bool triggered;

        protected virtual void Awake()
        {
            tutorialManager = tutorialManagerProvider as ITutorialManager;

            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (triggerOnce && triggered) return;
            if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;

            if (tutorialManager == null) return;

            triggered = true;
            tutorialManager?.NextStep();
        }
    }
}