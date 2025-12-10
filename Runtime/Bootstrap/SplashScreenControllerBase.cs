using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem;

namespace ErccDev.Foundation.Bootstrap
{
    /// <summary>
    /// Base logo flow: play video, allow optional skip, then load next scene.
    /// Subclass to set defaults or extend hooks.
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public abstract class SplashScreenControllerBase : MonoBehaviour
    {
        [Header("Next Scene")]
        [SerializeField] protected string nextSceneName = "Game";
        [Tooltip("If true, will use Foundation.Loader.SceneLoader if found.")]
        [SerializeField] protected bool preferSceneLoader = true;

        [Header("Playback")]
        [SerializeField] protected bool waitForPrepare = true;
        [SerializeField] protected bool waitForFirstFrame = true;
        [SerializeField, Min(0f)] protected float minDisplaySeconds = 0f;

        [Header("Skip (New Input System)")]
        [Tooltip("Optional input action to skip. If null, a default action can be created.")]
        [SerializeField] protected InputActionProperty skipAction;
        [SerializeField] protected bool createDefaultSkipAction = true;

        protected VideoPlayer VideoPlayer { get; private set; }
        protected bool Finished { get; private set; }
        protected float StartTime { get; private set; }
        protected bool OwnsAction { get; private set; }

        protected virtual void Awake()
        {
            VideoPlayer = GetComponent<VideoPlayer>();
            VideoPlayer.isLooping   = false;
            VideoPlayer.playOnAwake = false;
            VideoPlayer.skipOnDrop  = true;

            VideoPlayer.loopPointReached += OnVideoFinished;
            VideoPlayer.errorReceived     += OnVideoError;

            EnsureSkipAction();
        }

        protected virtual IEnumerator Start()
        {
            bool hasVideo = VideoPlayer.clip != null || !string.IsNullOrEmpty(VideoPlayer.url);
            StartTime = Time.unscaledTime;

            if (!hasVideo)
            {
                if (minDisplaySeconds > 0f)
                    yield return new WaitForSecondsRealtime(minDisplaySeconds);
                Finish();
                yield break;
            }

            if (waitForPrepare)
            {
                VideoPlayer.Prepare();
                while (!VideoPlayer.isPrepared) yield return null;
            }

            VideoPlayer.Play();

            if (waitForFirstFrame)
            {
                while (VideoPlayer.isPlaying && VideoPlayer.frame <= 0) yield return null;
            }
        }

        protected virtual void OnEnable()
        {
            if (skipAction.action != null)
            {
                skipAction.action.performed += OnSkipPerformed;
                if (!skipAction.action.enabled) skipAction.action.Enable();
            }
        }

        protected virtual void OnDisable()
        {
            if (skipAction.action != null)
            {
                skipAction.action.performed -= OnSkipPerformed;
                if (OwnsAction) skipAction.action.Disable();
            }
        }

        protected virtual void OnDestroy()
        {
            VideoPlayer.loopPointReached -= OnVideoFinished;
            VideoPlayer.errorReceived     -= OnVideoError;

            if (OwnsAction && skipAction.action != null)
            {
                skipAction.action.Dispose();
            }
        }

        // Public API
        public virtual void Skip() => Finish();

        protected virtual void EnsureSkipAction()
        {
            if (skipAction.action != null || !createDefaultSkipAction) return;

            var a = new InputAction("Skip", InputActionType.Button);
            a.AddBinding("<Keyboard>/space");
            a.AddBinding("<Gamepad>/start");
            a.AddBinding("<Mouse>/leftButton");
            a.AddBinding("<Touchscreen>/primaryTouch/press");
            skipAction = new InputActionProperty(a);
            OwnsAction = true;
        }

        protected virtual void OnSkipPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) Skip();
        }

        protected virtual void OnVideoFinished(VideoPlayer _) => Finish();

        protected virtual void OnVideoError(VideoPlayer _, string msg)
        {
            Debug.LogWarning($"[LogoScene] Video error: {msg}");
            Finish();
        }

        protected virtual void Finish()
        {
            if (Finished) return;
            Finished = true;

            float shown = Time.unscaledTime - StartTime;
            float remain = Mathf.Max(0f, minDisplaySeconds - shown);
            if (remain > 0f)
            {
                StartCoroutine(LoadAfterDelay(remain));
            }
            else
            {
                LoadNow();
            }
        }

        protected virtual IEnumerator LoadAfterDelay(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            LoadNow();
        }

        protected virtual void LoadNow()
        {
            if (preferSceneLoader)
            {
                var loader = FindAnyObjectByType<Foundation.Loader.SceneLoader>(FindObjectsInactive.Exclude);
                if (loader != null)
                {
                    loader.LoadSceneAsync(nextSceneName, additive: false);
                    return;
                }
            }
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
    }
}
