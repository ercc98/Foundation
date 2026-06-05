using UnityEngine;
using UnityEngine.InputSystem;

namespace ErccDev.Foundation.Input.Touch
{
    [CreateAssetMenu(menuName = "ErccDev/Input/Steering Touch Input Config")]
    public class SteeringTouchInputConfig : ScriptableObject
    {
        [Header("Input Actions (New Input System)")]
        public InputActionReference pointerPosition;
        public InputActionReference pointerPress;

        [Header("Mode")]
        [Tooltip("If true, steering is based on absolute screen position. If false, steering is based on drag delta from the initial touch point.")]
        public bool useAbsoluteScreenPosition = true;

        [Header("Absolute Mode")]
        [Range(0f, 0.5f)] public float centerDeadZoneNormalizedX = 0.05f;
        [Range(0f, 0.5f)] public float centerDeadZoneNormalizedY = 0.05f;

        [Header("Relative Mode (Horizontal)")]
        [Min(0f)] public float minDragPixelsX = 10f;
        [Min(1f)] public float maxDragPixelsX = 200f;

        [Header("Relative Mode (Vertical)")]
        [Min(0f)] public float minDragPixelsY = 10f;
        [Min(1f)] public float maxDragPixelsY = 200f;
    }
}
