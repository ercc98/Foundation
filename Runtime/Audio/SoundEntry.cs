using System;
using UnityEngine;

namespace ErccDev.Foundation.Audio
{
    [Serializable]
    public class SoundEntry
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Header("Optional randomization (SFX)")]
        [Range(0f, 0.5f)] public float volumeJitter = 0f; // +/- applied
        [Range(0f, 1f)]   public float pitchJitter  = 0f; // +/- around 1.0
    }
}