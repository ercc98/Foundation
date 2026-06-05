using UnityEngine;

namespace ErccDev.Foundation.Input.Pinch
{
    [CreateAssetMenu(menuName = "ErccDev/Input/Pinch Input Config")]
    public class PinchInputConfig : ScriptableObject
    {
        [Header("Threshold")]
        [Tooltip("Minimum change in finger distance (in DPI-scaled pixels) before a PinchedIn/PinchedOut event fires. Smooths out jitter.")]
        [Min(0f)] public float pinchThresholdPixels = 5f;

        [Header("Behaviour")]
        [Tooltip("When more than two fingers touch the screen, keep tracking the first two instead of cancelling the pinch.")]
        public bool lockToFirstTwoFingers = true;
    }
}
