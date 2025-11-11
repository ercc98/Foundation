using UnityEngine;
using UnityEngine.InputSystem;

namespace ErccDev.Input
{
    [CreateAssetMenu(menuName = "ErccDev/Input/Swipe Input Config")]
    public class SwipeInputConfig : ScriptableObject
    {
        [Header("Input Actions (New Input System)")]
        public InputActionReference pointerPosition; 
        public InputActionReference pointerPress;    

        [Header("Thresholds (screen pixels, time in seconds)")]
        [Min(0f)] public float minSwipePixels = 50f;
        [Min(0f)] public float tapMaxPixels = 20f;
        [Min(0f)] public float tapMaxTime = 0.25f;
    }
}