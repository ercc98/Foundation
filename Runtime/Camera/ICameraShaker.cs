
using UnityEngine;

namespace ErccDev.Foundation.Camera
{
    public interface ICameraShaker
    {
        void Shake(float amplitude, float frequency, float duration);
        void Shake(CameraShakeProfile profile);
        void StopShake();

        float IntensityMultiplier { get; set; }
        bool  IsShaking { get; }
    }
}