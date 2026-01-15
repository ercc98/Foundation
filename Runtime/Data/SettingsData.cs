using System;
using UnityEngine;

namespace ErccDev.Foundation.Data
{
    [CreateAssetMenu(fileName = "SettingsData", menuName = "ErccDev/Settings/Settings Data")]
    public sealed class SettingsData : ScriptableObject
    {
        [Header("Audio")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float ambientVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float musicVolume  = 1f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float uiVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float voiceVolume = 1f;


        [Header("Audio Toggles")]
        [SerializeField] private bool musicEnabled   = true;
        [SerializeField] private bool ambientEnabled = true;
        [SerializeField] private bool sfxEnabled     = true;
        [SerializeField] private bool uiEnabled      = true;
        [SerializeField] private bool voiceEnabled   = true;

        [Header("Gameplay")]
        [SerializeField] private bool vibration = true;

        [Header("Graphics")]
        [SerializeField, Range(0, 5)] private int qualityLevel = 2;

        public float MasterVolume { get => masterVolume; set => masterVolume = Mathf.Clamp01(value); }
        public float AmbientVolume { get => ambientVolume; set => ambientVolume = Mathf.Clamp01(value); }
        public float MusicVolume    { get => musicVolume;  set => musicVolume  = Mathf.Clamp01(value); }
        public float SfxVolume      { get => sfxVolume; set => sfxVolume = Mathf.Clamp01(value); }
        public float UiVolume       { get => uiVolume; set => uiVolume = Mathf.Clamp01(value); }
        public float VoiceVolume    { get => voiceVolume; set => voiceVolume = Mathf.Clamp01(value); }

        public bool MusicEnabled   { get => musicEnabled;   set => musicEnabled   = value; }
        public bool AmbientEnabled { get => ambientEnabled; set => ambientEnabled = value; }
        public bool SfxEnabled     { get => sfxEnabled;     set => sfxEnabled     = value; }
        public bool UiEnabled      { get => uiEnabled;      set => uiEnabled      = value; }
        public bool VoiceEnabled   { get => voiceEnabled;   set => voiceEnabled   = value; }

        public bool Vibration { get => vibration; set => vibration = value; }

        public int QualityLevel { get => qualityLevel; set => qualityLevel = Mathf.Clamp(value, 0, 5); }

        public void Clamp()
        {
            masterVolume = Mathf.Clamp01(masterVolume);
            musicVolume  = Mathf.Clamp01(musicVolume);
            sfxVolume    = Mathf.Clamp01(sfxVolume);
            qualityLevel = Mathf.Clamp(qualityLevel, 0, 5);
        }
    }
}
