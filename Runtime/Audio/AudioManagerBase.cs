using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;

namespace ErccDev.Foundation.Audio
{
    public abstract class AudioManagerBase : MonoBehaviour, IAudioService
    {
        [Header("Sound Groups (ScriptableObjects)")]
        public SoundGroup ambientGroup;
        public SoundGroup musicGroup;
        public SoundGroup sfxGroup;
        public SoundGroup uiGroup;
        public SoundGroup voiceGroup;

        [Header("Main Audio Sources (one per category)")]
        public AudioSource ambientSource;
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource uiSource;
        public AudioSource voiceSource;

        [Header("SFX Voice Pool (optional overlap)")]
        [SerializeField] private bool useSfxPool = true;
        [SerializeField, Min(1)] private int sfxVoices = 8;
        private readonly List<AudioSource> _sfxPool = new();
        private int _sfxIndex;

        [Header("Audio Mixer (optional)")]
        [Tooltip("Expose params named: AmbientVolume, MusicVolume, SFXVolume, UIVolume, VoiceVolume")]
        public AudioMixer audioMixer;

        // Track current loop per category for precise StopLoop(id)
        private readonly Dictionary<SoundCategory, AudioClip> _currentLoop = new();

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ambientGroup?.Init();
            musicGroup?.Init();
            sfxGroup?.Init();
            uiGroup?.Init();
            voiceGroup?.Init();

            if (useSfxPool && sfxSource != null)
                BuildSfxPool();

            // Restore toggles/volumes
            LoadPersistedSettings();
        }

        // ---------------- IAudioService ----------------

        public void Play(string id, SoundCategory category)
        {
            var entry  = GetGroup(category)?.Get(id);
            var source = GetSource(category);
            if (entry?.clip == null || source == null)
            {
                Debug.LogWarning($"[Audio] '{id}' not found in {category}.");
                return;
            }

            if (category == SoundCategory.SFX && useSfxPool && _sfxPool.Count > 0)
            {
                var voice = NextSfxVoice();
                ApplyJitter(voice, entry);
                voice.PlayOneShot(entry.clip, voice.volume);
            }
            else
            {
                ApplyJitter(source, entry);
                source.PlayOneShot(entry.clip, source.volume);
            }
        }

        public void Play(Enum id, SoundCategory category) => Play(id.ToString(), category);

        public void PlayLoop(string id, SoundCategory category)
        {
            var entry  = GetGroup(category)?.Get(id);
            var source = GetSource(category);
            if (entry?.clip == null || source == null)
            {
                Debug.LogWarning($"[Audio] Loop '{id}' not found in {category}.");
                return;
            }

            source.Stop();
            source.clip   = entry.clip;
            source.volume = Mathf.Clamp01(entry.volume);
            source.loop   = true;
            source.Play();

            _currentLoop[category] = entry.clip;
        }

        public void PlayLoop(Enum id, SoundCategory category) => PlayLoop(id.ToString(), category);

        public virtual void StopLoop(SoundCategory category)
        {
            var src = GetSource(category);
            if (src != null && src.isPlaying && src.loop) src.Stop();
            _currentLoop.Remove(category);
        }

        public void StopLoop(Enum id, SoundCategory category)
        {
            var src = GetSource(category);
            var entry = GetGroup(category)?.Get(id.ToString());
            if (src != null && entry?.clip != null &&
                _currentLoop.TryGetValue(category, out var clip) && clip == entry.clip)
            {
                src.Stop();
                _currentLoop.Remove(category);
            }
        }

        public void SetVolume(SoundCategory category, float volume01)
        {
            PlayerPrefs.SetFloat($"audio_{category}_vol", Mathf.Clamp01(volume01));
            float dB = Mathf.Log10(Mathf.Clamp(volume01, 0.0001f, 1f)) * 20f;
            audioMixer?.SetFloat($"{category}Volume", dB);
        }

        public void Toggle(SoundCategory category, bool isOn)
        {
            PlayerPrefs.SetInt($"audio_{category}", isOn ? 1 : 0);
            PlayerPrefs.Save();

            // Apply: set to stored vol or 0
            float vol = isOn ? PlayerPrefs.GetFloat($"audio_{category}_vol", 1f) : 0f;
            SetVolume(category, vol);
        }

        // ---------------- Helpers ----------------

        protected AudioSource GetSource(SoundCategory c) => c switch
        {
            SoundCategory.Ambient => ambientSource,
            SoundCategory.Music   => musicSource,
            SoundCategory.SFX     => sfxSource,
            SoundCategory.UI      => uiSource,
            SoundCategory.Voice   => voiceSource,
            _ => null
        };

        protected SoundGroup GetGroup(SoundCategory c) => c switch
        {
            SoundCategory.Ambient => ambientGroup,
            SoundCategory.Music   => musicGroup,
            SoundCategory.SFX     => sfxGroup,
            SoundCategory.UI      => uiGroup,
            SoundCategory.Voice   => voiceGroup,
            _ => null
        };

        void BuildSfxPool()
        {
            Transform parent = sfxSource.transform.parent ?? transform;
            for (int i = 0; i < sfxVoices; i++)
            {
                var v = Instantiate(sfxSource, parent);
                v.name = $"SFXVoice_{i}";
                v.loop = false;
                _sfxPool.Add(v);
            }
        }

        AudioSource NextSfxVoice()
        {
            _sfxIndex = (_sfxIndex + 1) % _sfxPool.Count;
            return _sfxPool[_sfxIndex];
        }

        static void ApplyJitter(AudioSource src, SoundEntry e)
        {
            if (!src || e == null) return;
            float v = e.volume + UnityEngine.Random.Range(-e.volumeJitter, e.volumeJitter);
            float p = 1f        + UnityEngine.Random.Range(-e.pitchJitter,  e.pitchJitter);
            src.volume = Mathf.Clamp01(v);
            src.pitch  = Mathf.Clamp(p, 0.5f, 2f);
        }

        void LoadPersistedSettings()
        {
            foreach (SoundCategory cat in Enum.GetValues(typeof(SoundCategory)))
            {
                float vol = PlayerPrefs.GetFloat($"audio_{cat}_vol", 1f);
                bool  on  = PlayerPrefs.GetInt($"audio_{cat}", 1) == 1;
                SetVolume(cat, on ? vol : 0f);
            }
        }
    }
}