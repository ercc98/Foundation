using UnityEngine;
using UnityEngine.InputSystem;

namespace ErccDev.Foundation.Input.Touch
{
    [CreateAssetMenu(menuName = "ErccDev/Input/Horizontal Touch Input Config")]
    public class HorizontalTouchInputConfig : ScriptableObject
    {
        [Header("Input Actions (New Input System)")]
        public InputActionReference pointerPosition;
        public InputActionReference pointerPress;

        [Header("Mode")]
        [Tooltip("If true, SteeringX is based on absolute screen position. If false, SteeringX is based on drag delta from the initial touch point.")]
        public bool useAbsoluteScreenPosition = true;

        [Header("Absolute Mode")]
        [Range(0f, 0.5f)]
        public float centerDeadZoneNormalized = 0.05f;

        [Header("Relative Mode")]
        [Min(0f)] public float minDragPixels = 10f;
        [Min(1f)] public float maxDragPixels = 200f;
    }
}