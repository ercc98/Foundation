using UnityEngine;

namespace ErccDev.Foundation.Data
{
    [CreateAssetMenu(
        fileName = "SettingsData",
        menuName = "ErccDev/Foundation/Settings Data"
    )]
    public sealed class SettingsData : ScriptableObject
    {
        [Header("Audio")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float musicVolume  = 1f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume    = 1f;

        [Header("Gameplay")]
        [SerializeField] private bool vibration = true;

        [Header("Graphics")]
        [SerializeField, Range(0, 5)] private int qualityLevel = 2;


        public float MasterVolume { get => masterVolume; set => masterVolume = Mathf.Clamp01(value); }
        public float MusicVolume { get => musicVolume; set => musicVolume = Mathf.Clamp01(value); }
        public float SfxVolume { get => sfxVolume; set => sfxVolume = Mathf.Clamp01(value); }
        public bool Vibration { get => vibration; set => vibration = value; }
        public int QualityLevel { get => qualityLevel; set => qualityLevel = Mathf.Clamp(value, 0, 5); }

        public void Clamp()
        {
            masterVolume = Mathf.Clamp01(masterVolume);
            musicVolume  = Mathf.Clamp01(musicVolume);
            sfxVolume    = Mathf.Clamp01(sfxVolume);
            qualityLevel = Mathf.Clamp(qualityLevel, 0, 5);
        }

        public void Apply()
        {
            Clamp();
            AudioListener.volume = masterVolume;
            QualitySettings.SetQualityLevel(qualityLevel, applyExpensiveChanges: true);
        }
    }
}
