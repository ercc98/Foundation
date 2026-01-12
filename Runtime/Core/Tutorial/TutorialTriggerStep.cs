using UnityEngine;

namespace ErccDev.Foundation.Core.Tutorial
{
    [RequireComponent(typeof(Collider))]
    public sealed class TutorialTriggerStep : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private MonoBehaviour tutorialManagerProvider;
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private string requiredTag = "Player";

        private ITutorialManager tutorialManager;
        private bool triggered;

        private void Awake()
        {
            tutorialManager = tutorialManagerProvider as ITutorialManager;

            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnce && triggered) return;
            if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;

            if (tutorialManager == null) return;

            triggered = true;
            tutorialManager?.NextStep();
        }
    }
}