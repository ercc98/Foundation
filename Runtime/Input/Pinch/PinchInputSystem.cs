using System;
using ErccDev.Foundation.Core.Input;
using UnityEngine;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

namespace ErccDev.Foundation.Input.Pinch
{
    /// <summary>
    /// Two-finger pinch input. Polls the two active fingers each frame (EnhancedTouch) and raises
    /// <see cref="PinchedIn"/> / <see cref="PinchedOut"/> as they move closer or apart. Extra fingers
    /// are ignored (the first two are tracked) when <see cref="PinchInputConfig.lockToFirstTwoFingers"/>.
    /// </summary>
    public class PinchInputSystem : InputModule<PinchInputConfig>, IPinchInput
    {
        public event Action PinchStarted;
        public event Action PinchEnded;
        public event Action PinchedIn;
        public event Action PinchedOut;

        public bool IsPinching => _isPinching;

        public float Distance      => _currentDistance;
        public float StartDistance => _startDistance;
        public float DeltaPixels   => _deltaPixels;
        public float Scale         => _startDistance > 0f ? _currentDistance / _startDistance : 1f;
        public Vector2 Midpoint    => _midpoint;

        bool    _isPinching;
        float   _startDistance;
        float   _currentDistance;
        float   _previousDistance;
        float   _deltaPixels;
        Vector2 _midpoint;

        // EnhancedTouchSupport is reference-counted, so paired Enable/Disable plays nice with other modules.
        public override void EnableModule()  => ETouch.EnhancedTouchSupport.Enable();
        public override void DisableModule() => ETouch.EnhancedTouchSupport.Disable();

        protected override void ValidateOrWarn()
        {
#if UNITY_EDITOR
            if (config == null)
                Debug.LogWarning($"[{nameof(PinchInputSystem)}] Missing config ScriptableObject.", this);
#endif
        }

        void Update()
        {
            if (config == null)
                return;

            var touches = ETouch.Touch.activeTouches;

            bool tooFew  = touches.Count < 2;
            bool tooMany = touches.Count > 2 && !config.lockToFirstTwoFingers;

            if (tooFew || tooMany)
            {
                if (_isPinching)
                    EndPinch();
                return;
            }

            EvaluatePinch(touches[0].screenPosition, touches[1].screenPosition);
        }

        // Drives the pinch from two screen-space finger positions. Isolated so it can be unit-driven.
        void EvaluatePinch(Vector2 fingerA, Vector2 fingerB)
        {
            float distance = Vector2.Distance(fingerA, fingerB);
            _midpoint = (fingerA + fingerB) * 0.5f;

            if (!_isPinching)
            {
                BeginPinch(distance);
                return;
            }

            _currentDistance = distance;
            _deltaPixels     = _currentDistance - _previousDistance;

            float threshold = config.pinchThresholdPixels * DpiScale;

            if (_deltaPixels > threshold)
            {
                _previousDistance = _currentDistance;
                PinchedOut?.Invoke();
            }
            else if (_deltaPixels < -threshold)
            {
                _previousDistance = _currentDistance;
                PinchedIn?.Invoke();
            }
        }

        void BeginPinch(float distance)
        {
            _isPinching       = true;
            _startDistance    = distance;
            _currentDistance  = distance;
            _previousDistance = distance;
            _deltaPixels      = 0f;

            PinchStarted?.Invoke();
        }

        void EndPinch()
        {
            _isPinching  = false;
            _deltaPixels = 0f;

            PinchEnded?.Invoke();
        }
    }
}
