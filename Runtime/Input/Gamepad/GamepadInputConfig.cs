using UnityEngine;
using UnityEngine.InputSystem;

namespace ErccDev.Foundation.Input.Gamepad
{
    [CreateAssetMenu(menuName = "ErccDev/Input/Gamepad Input Config")]
    public class GamepadInputConfig : ScriptableObject
    {
        [Header("Sticks (Vector2 actions)")]
        public InputActionReference moveAction;   // left stick
        public InputActionReference lookAction;   // right stick

        [Header("Face Buttons (Button actions)")]
        public InputActionReference buttonSouth;
        public InputActionReference buttonEast;
        public InputActionReference buttonWest;
        public InputActionReference buttonNorth;

        [Header("Shoulders (Button actions)")]
        public InputActionReference leftShoulder;
        public InputActionReference rightShoulder;

        [Header("Triggers (float actions)")]
        public InputActionReference leftTrigger;
        public InputActionReference rightTrigger;

        [Header("D-Pad (Vector2 action)")]
        public InputActionReference dpad;

        [Header("Tuning")]
        [Tooltip("Radial dead zone applied to each analog stick. Magnitudes below this read as zero.")]
        [Range(0f, 1f)] public float stickDeadZone = 0.15f;

        [Tooltip("Triggers below this value are reported as 0 (and not 'held').")]
        [Range(0f, 1f)] public float triggerThreshold = 0.1f;

        [Tooltip("D-pad axis magnitude above which a direction is considered active.")]
        [Range(0f, 1f)] public float dpadThreshold = 0.5f;
    }
}
