using System;
using ErccDev.Foundation.Core.Input;
using ErccDev.Foundation.Input.Swipe;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ErccDev.Foundation.Input.Touch
{
    public class HorizontalTouchInputSystem : InputModule<HorizontalTouchInputConfig>, IHorizontalTouchInput, ITouchInput
    {
        public event Action TouchStarted;
        public event Action TouchEnded;
        public event Action MovedLeft;
        public event Action MovedRight;
        public event Action ReturnedToCenter;

        public event Action StartTouch;
        public event Action EndTouch;

        public bool IsTouching => _isPressing;
        public Vector2 PointerPosition => _pointerPosition;
        public Vector2 StartPosition => _startPosition;

        public float NormalizedX
        {
            get
            {
                if (Screen.width <= 0)
                    return 0.5f;

                return Mathf.Clamp01(_pointerPosition.x / Screen.width);
            }
        }

        public float SteeringX
        {
            get
            {
                if (!_isPressing || config == null)
                    return 0f;

                return config.useAbsoluteScreenPosition
                    ? GetAbsoluteSteeringX()
                    : GetRelativeSteeringX();
            }
        }

        public bool IsLeftSide => SteeringX < 0f;
        public bool IsRightSide => SteeringX > 0f;
        public bool IsCenter => Mathf.Approximately(SteeringX, 0f);

        Vector2 _pointerPosition;
        Vector2 _startPosition;
        bool _isPressing;

        HorizontalState _currentState = HorizontalState.Center;

        enum HorizontalState
        {
            Center,
            Left,
            Right
        }

        public override void EnableModule()
        {
            if (config?.pointerPress != null)
            {
                var press = config.pointerPress.action;
                press.started += OnPressStarted;
                press.canceled += OnPressCanceled;
                press.Enable();
            }

            if (config?.pointerPosition != null)
            {
                var position = config.pointerPosition.action;
                position.performed += OnPointerMoved;
                position.canceled += OnPointerMoved;
                position.Enable();
            }
        }

        public override void DisableModule()
        {
            if (config?.pointerPress != null)
            {
                var press = config.pointerPress.action;
                press.started -= OnPressStarted;
                press.canceled -= OnPressCanceled;
                press.Disable();
            }

            if (config?.pointerPosition != null)
            {
                var position = config.pointerPosition.action;
                position.performed -= OnPointerMoved;
                position.canceled -= OnPointerMoved;
                position.Disable();
            }
        }

        protected override void ValidateOrWarn()
        {
#if UNITY_EDITOR
            if (config == null)
                Debug.LogWarning($"[{nameof(HorizontalTouchInputSystem)}] Missing config ScriptableObject.", this);
#endif
        }

        protected override void OnAfterConfigChange()
        {
            if (!isActiveAndEnabled)
                return;

            DisableModule();
            EnableModule();
        }

        void OnPressStarted(InputAction.CallbackContext ctx)
        {
            _isPressing = true;
            _pointerPosition = ReadPointer();
            _startPosition = _pointerPosition;
            _currentState = HorizontalState.Center;

            TouchStarted?.Invoke();
            StartTouch?.Invoke();

            EvaluateHorizontalState();
        }

        void OnPressCanceled(InputAction.CallbackContext ctx)
        {
            if (!_isPressing)
                return;

            _pointerPosition = ReadPointer();
            _isPressing = false;

            if (_currentState != HorizontalState.Center)
            {
                _currentState = HorizontalState.Center;
                ReturnedToCenter?.Invoke();
            }

            TouchEnded?.Invoke();
            EndTouch?.Invoke();
        }

        void OnPointerMoved(InputAction.CallbackContext ctx)
        {
            _pointerPosition = ctx.ReadValue<Vector2>();

            if (_isPressing)
                EvaluateHorizontalState();
        }

        void EvaluateHorizontalState()
        {
            HorizontalState newState = GetStateFromSteering();

            if (newState == _currentState)
                return;

            _currentState = newState;

            switch (_currentState)
            {
                case HorizontalState.Left:
                    MovedLeft?.Invoke();
                    break;

                case HorizontalState.Right:
                    MovedRight?.Invoke();
                    break;

                default:
                    ReturnedToCenter?.Invoke();
                    break;
            }
        }

        HorizontalState GetStateFromSteering()
        {
            float steer = SteeringX;

            if (steer < 0f) return HorizontalState.Left;
            if (steer > 0f) return HorizontalState.Right;
            return HorizontalState.Center;
        }

        Vector2 ReadPointer()
        {
            return config?.pointerPosition != null
                ? config.pointerPosition.action.ReadValue<Vector2>()
                : Vector2.zero;
        }

        float GetAbsoluteSteeringX()
        {
            float centered = (NormalizedX - 0.5f) * 2f;
            float deadZone = Mathf.Clamp01(config.centerDeadZoneNormalized) * 2f;

            if (Mathf.Abs(centered) <= deadZone)
                return 0f;

            float sign = Mathf.Sign(centered);
            float magnitude = Mathf.InverseLerp(deadZone, 1f, Mathf.Abs(centered));
            return sign * magnitude;
        }

        float GetRelativeSteeringX()
        {
            float minDrag = config.minDragPixels * DpiScale;
            float maxDrag = Mathf.Max(minDrag + 1f, config.maxDragPixels * DpiScale);

            float deltaX = _pointerPosition.x - _startPosition.x;
            float absDelta = Mathf.Abs(deltaX);

            if (absDelta <= minDrag)
                return 0f;

            float sign = Mathf.Sign(deltaX);
            float magnitude = Mathf.InverseLerp(minDrag, maxDrag, absDelta);
            return sign * magnitude;
        }
    }
}