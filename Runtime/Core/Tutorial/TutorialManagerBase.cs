using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Tutorial
{
    public class TutorialManager : MonoBehaviour, ITutorialManager
    {
        [Header("Setup")]
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private MonoBehaviour uiProvider;
        [SerializeField] private GameObject tutorialMoverPrefab;
             
        private int currentIndex;
        private bool currentStepCompleted = false;
        private readonly float stepDelay = 6f;
        private Coroutine startedCoroutine;
        private TutorialStep currentStep;
        

        private ITutorialStepUI ui;
        protected ITutorialContext Context { get; private set; }

        public bool IsRunning => currentStep != null;
        public event Action OnTutorialEnded;   

        private void Awake()
        {
            ui = uiProvider as ITutorialStepUI;
        }

        public void SetContext(ITutorialContext ctx) => Context = ctx;

        public void StartTutorial()
        {
            tutorialMoverPrefab.SetActive(true);
            currentIndex = 0;
        }

        private void StartStep()
        {
            currentStepCompleted = false;
            currentStep = steps[currentIndex];
            currentStep.Initialize(Context);
            ui.Show(currentStep);
            Time.timeScale = 0f; // Pause game during tutorial
        }

        private void Update()
        {
            if (currentStep == null) return;

            if (!currentStepCompleted && currentStep.IsCompleted())
                CompleteStep();

        }

        private void CompleteStep()
        {
            currentStep.Cleanup();
            ui.Hide();

            currentIndex++;
            Debug.Log("Step completed");
            Time.timeScale = 1f; // Resume game after tutorial step
            currentStepCompleted = true;
            if (currentIndex >= steps.Count)
            {
                EndTutorial();
                return;
            }

        }

        private void EndTutorial()
        {
            Debug.Log("Tutorial completed");

            OnTutorialEnded?.Invoke();
            currentStep = null;
            startedCoroutine = StartCoroutine(DisableGameObjectCorroutine());
        }

        IEnumerator DisableGameObjectCorroutine()
        {
            yield return new WaitForSeconds(stepDelay);
            tutorialMoverPrefab.SetActive(false);
            startedCoroutine = null;
            gameObject.SetActive(false);
        }

        public void SkipTutorial()
        {
            currentStep?.Cleanup();
            ui.Hide();
            EndTutorial();
        }

        public void NextStep()
        {
            StartStep();
        }

        void OnDisable()
        {
            if (startedCoroutine != null)
            {
                StopCoroutine(startedCoroutine);
                startedCoroutine = null;
            }
        }
    }
}