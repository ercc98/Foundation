// Assets/Scripts/ErccDev/Foundation/Cameras/CameraShakeProfile.cs
using UnityEngine;

namespace ErccDev.Foundation.Camera
{
    [CreateAssetMenu(fileName = "CameraShakeProfile", menuName = "ErccDev/Camera/Shake Profile")]
    public sealed class CameraShakeProfile : ScriptableObject
    {
        [Range(0f, 5f)]  public float amplitude = 1f;
        [Range(0f, 10f)] public float frequency = 2f;
        [Min(0f)]        public float duration  = 0.25f;
        public bool      vibrateMobile = false;
    }
}
