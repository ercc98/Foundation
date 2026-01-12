using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Tutorial
{
    public class TutorialManagerBase : MonoBehaviour, ITutorialManager
    {
        [Header("Setup")]
        [SerializeField] protected List<TutorialStep> steps;
        [SerializeField] protected MonoBehaviour uiProvider;
        [SerializeField] protected GameObject tutorialMoverPrefab;
             
        protected int currentIndex;
        protected bool currentStepCompleted = false;
        protected readonly float stepDelay = 6f;
        protected Coroutine startedCoroutine;
        protected TutorialStep currentStep;
        

        protected ITutorialStepUI ui;
        protected ITutorialContext Context { get; private set; }

        public bool IsRunning => currentStep != null;
        public event Action OnTutorialEnded;   

        protected void Awake()
        {
            ui = uiProvider as ITutorialStepUI;
        }

        public void SetContext(ITutorialContext ctx) => Context = ctx;

        public void StartTutorial()
        {
            tutorialMoverPrefab.SetActive(true);
            currentIndex = 0;
        }

        protected void StartStep()
        {
            currentStepCompleted = false;
            currentStep = steps[currentIndex];
            currentStep.Initialize(Context);
            ui.Show(currentStep);
            Time.timeScale = 0f; // Pause game during tutorial
        }

        protected void Update()
        {
            if (currentStep == null) return;

            if (!currentStepCompleted && currentStep.IsCompleted())
                CompleteStep();

        }

        protected void CompleteStep()
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

        protected void EndTutorial()
        {
            Debug.Log("Tutorial completed");

            OnTutorialEnded?.Invoke();
            currentStep = null;
            startedCoroutine = StartCoroutine(DisableGameObjectCorroutine());
        }

        protected IEnumerator DisableGameObjectCorroutine()
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

        protected void OnDisable()
        {
            if (startedCoroutine != null)
            {
                StopCoroutine(startedCoroutine);
                startedCoroutine = null;
            }
        }
    }
}