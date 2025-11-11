using System;

namespace ErccDev.Audio
{
    public interface IAudioService
    {
        void Play(string id, SoundCategory category);
        void Play(Enum id,  SoundCategory category); // convenience
        void PlayLoop(string id, SoundCategory category);
        void PlayLoop(Enum id,  SoundCategory category);
        void StopLoop(SoundCategory category);
        void StopLoop(Enum id,  SoundCategory category); // precise stop when IDs matter
        void SetVolume(SoundCategory category, float volume01);
        void Toggle(SoundCategory category, bool isOn);
    }
}