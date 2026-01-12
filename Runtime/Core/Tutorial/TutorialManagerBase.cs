using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Tutorial
{
    public class TutorialManagerBase : MonoBehaviour, ITutorialManager, ITutorialManagerConfig
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

        protected virtual void Awake()
        {
            ui = uiProvider as ITutorialStepUI;
        }

        public virtual void SetContext(ITutorialContext ctx) => Context = ctx;

        public virtual void StartTutorial()
        {
            tutorialMoverPrefab.SetActive(true);
            currentIndex = 0;
        }

        protected virtual void StartStep()
        {
            currentStepCompleted = false;
            currentStep = steps[currentIndex];
            currentStep.Initialize(Context);
            ui.Show(currentStep);
        }

        protected virtual void Update()
        {
            if (currentStep == null) return;

            if (!currentStepCompleted && currentStep.IsCompleted())
                CompleteStep();

        }

        protected virtual void CompleteStep()
        {
            currentStep.Cleanup();
            ui.Hide();

            currentIndex++;            
            currentStepCompleted = true;
            if (currentIndex >= steps.Count)
            {
                EndTutorial();
                return;
            }

        }

        protected virtual void EndTutorial()
        {
            OnTutorialEnded?.Invoke();
            currentStep = null;
            startedCoroutine = StartCoroutine(DisableGameObjectCorroutine());
        }

        protected virtual IEnumerator DisableGameObjectCorroutine()
        {
            yield return new WaitForSeconds(stepDelay);
            tutorialMoverPrefab.SetActive(false);
            startedCoroutine = null;
            gameObject.SetActive(false);
        }

        public virtual void SkipTutorial()
        {
            currentStep?.Cleanup();
            ui.Hide();
            EndTutorial();
        }

        public virtual void NextStep()
        {
            StartStep();
        }

        protected virtual void OnDisable()
        {
            if (startedCoroutine != null)
            {
                StopCoroutine(startedCoroutine);
                startedCoroutine = null;
            }
        }
    }
}