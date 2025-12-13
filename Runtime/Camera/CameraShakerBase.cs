// Assets/Scripts/ErccDev/Foundation/Cameras/CameraShaker.cs
using Unity.Cinemachine;
using UnityEngine;

namespace ErccDev.Foundation.Camera
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineCamera))]
    [RequireComponent(typeof(CinemachineBasicMultiChannelPerlin))]
    
    public class CameraShakerBase : MonoBehaviour, ICameraShaker
    {
        [Header("Binding")]
        [SerializeField] private CinemachineCamera cineCam;

        [Header("Defaults (fallback)")]
        [Range(0f, 5f)]  [SerializeField] private float defaultAmplitude = 1f;
        [Range(0f, 10f)] [SerializeField] private float defaultFrequency = 2f;
        [Min(0f)]        [SerializeField] private float defaultDuration  = 0.25f;

        [Header("Controls")]
        [SerializeField, Tooltip("Scales amplitude & frequency for all shakes.")]
        private float intensityMultiplier = 1f;
        [SerializeField, Tooltip("Vibrate on mobile when shaking (or use profile flag).")]
        
#if UNITY_ANDROID || UNITY_IOS
            private bool vibrateMobile = false;
#endif

        private CinemachineBasicMultiChannelPerlin perlin;
        private float timer;

        public float IntensityMultiplier { get => intensityMultiplier; set => intensityMultiplier = Mathf.Max(0f, value); }
        public bool  IsShaking => timer > 0f;

        void Reset()
        {
            cineCam = GetComponent<CinemachineCamera>();
        }

        void Awake()
        {
            if (!cineCam) cineCam = GetComponent<CinemachineCamera>();

            // CM3: noise lives as a component on the CinemachineCamera
            perlin = cineCam.GetComponent<CinemachineBasicMultiChannelPerlin>();

            ApplyNoise(0f, 0f);
            timer = 0f;
        }

        void Update()
        {
            if (timer <= 0f) return;
            timer -= Time.deltaTime;
            if (timer <= 0f) StopShake();
        }

        // -------- ICameraShaker --------
        public void Shake(CameraShakeProfile profile)
        {
            if (!profile)
            {
                Shake(defaultAmplitude, defaultFrequency, defaultDuration);
                return;
            }

            Shake(profile.amplitude, profile.frequency, profile.duration);

#if UNITY_ANDROID || UNITY_IOS
            if (profile.vibrateMobile || vibrateMobile) Handheld.Vibrate();
#endif
        }

        public void Shake(float amplitude, float frequency, float duration)
        {
            amplitude = Mathf.Max(0f, amplitude) * intensityMultiplier;
            frequency = Mathf.Max(0f, frequency) * Mathf.Max(0.01f, intensityMultiplier);
            duration  = Mathf.Max(0f, duration);

            ApplyNoise(amplitude, frequency);
            timer = duration;

#if UNITY_ANDROID || UNITY_IOS
            if (vibrateMobile) Handheld.Vibrate();
#endif
        }

        public void StopShake()
        {
            timer = 0f;
            ApplyNoise(0f, 0f);
        }
        // --------------------------------

        private void ApplyNoise(float amplitude, float frequency)
        {
            if (!perlin) return;
            perlin.AmplitudeGain = amplitude;
            perlin.FrequencyGain = frequency;
        }
    }
}
