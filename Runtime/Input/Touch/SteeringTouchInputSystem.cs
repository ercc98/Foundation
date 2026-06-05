using System;
using ErccDev.Foundation.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

namespace ErccDev.Foundation.Input.Touch
{
    [MovedFrom(true, sourceClassName: "HorizontalTouchInputSystem")]
    public class SteeringTouchInputSystem : InputModule<SteeringTouchInputConfig>, IHorizontalTouchInput, IVerticalTouchInput, ITouchInput
    {
        public event Action StartTouch;
        public event Action EndTouch;

        public event Action MovedLeft;
        public event Action MovedRight;
        public event Action ReturnedToCenterX;

        public event Action MovedUp;
        public event Action MovedDown;
        public event Action ReturnedToCenterY;

        public bool IsTouching => _isPressing;
        public Vector2 PointerPosition => _pointerPosition;
        public Vector2 StartPosition => _startPosition;

        // ---------- Horizontal ----------

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

        public bool IsLeftSide  => SteeringX < 0f;
        public bool IsRightSide => SteeringX > 0f;
        public bool IsCenterX   => Mathf.Approximately(SteeringX, 0f);

        // ---------- Vertical ----------

        public float NormalizedY
        {
            get
            {
                if (Screen.height <= 0)
                    return 0.5f;

                return Mathf.Clamp01(_pointerPosition.y / Screen.height);
            }
        }

        public float SteeringY
        {
            get
            {
                if (!_isPressing || config == null)
                    return 0f;

                return config.useAbsoluteScreenPosition
                    ? GetAbsoluteSteeringY()
                    : GetRelativeSteeringY();
            }
        }

        public bool IsTopSide    => SteeringY > 0f;
        public bool IsBottomSide => SteeringY < 0f;
        public bool IsCenterY    => Mathf.Approximately(SteeringY, 0f);

        Vector2 _pointerPosition;
        Vector2 _startPosition;
        bool _isPressing;

        AxisState _currentXState = AxisState.Center;
        AxisState _currentYState = AxisState.Center;

        enum AxisState
        {
            Center,
            Negative,
            Positive
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
                Debug.LogWarning($"[{nameof(SteeringTouchInputSystem)}] Missing config ScriptableObject.", this);
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
            _currentXState = AxisState.Center;
            _currentYState = AxisState.Center;

            StartTouch?.Invoke();

            EvaluateHorizontalState();
            EvaluateVerticalState();
        }

        void OnPressCanceled(InputAction.CallbackContext ctx)
        {
            if (!_isPressing)
                return;

            _pointerPosition = ReadPointer();
            _isPressing = false;

            if (_currentXState != AxisState.Center)
            {
                _currentXState = AxisState.Center;
                ReturnedToCenterX?.Invoke();
            }

            if (_currentYState != AxisState.Center)
            {
                _currentYState = AxisState.Center;
                ReturnedToCenterY?.Invoke();
            }

            EndTouch?.Invoke();
        }

        void OnPointerMoved(InputAction.CallbackContext ctx)
        {
            _pointerPosition = ctx.ReadValue<Vector2>();

            if (!_isPressing)
                return;

            EvaluateHorizontalState();
            EvaluateVerticalState();
        }

        void EvaluateHorizontalState()
        {
            AxisState newState = GetStateFromSteering(SteeringX);

            if (newState == _currentXState)
                return;

            _currentXState = newState;

            switch (_currentXState)
            {
                case AxisState.Negative:
                    MovedLeft?.Invoke();
                    break;

                case AxisState.Positive:
                    MovedRight?.Invoke();
                    break;

                default:
                    ReturnedToCenterX?.Invoke();
                    break;
            }
        }

        void EvaluateVerticalState()
        {
            AxisState newState = GetStateFromSteering(SteeringY);

            if (newState == _currentYState)
                return;

            _currentYState = newState;

            switch (_currentYState)
            {
                case AxisState.Positive:
                    MovedUp?.Invoke();
                    break;

                case AxisState.Negative:
                    MovedDown?.Invoke();
                    break;

                default:
                    ReturnedToCenterY?.Invoke();
                    break;
            }
        }

        static AxisState GetStateFromSteering(float steer)
        {
            if (steer < 0f) return AxisState.Negative;
            if (steer > 0f) return AxisState.Positive;
            return AxisState.Center;
        }

        Vector2 ReadPointer()
        {
            return config?.pointerPosition != null
                ? config.pointerPosition.action.ReadValue<Vector2>()
                : Vector2.zero;
        }

        float GetAbsoluteSteeringX() => AbsoluteSteering(NormalizedX, config.centerDeadZoneNormalizedX);
        float GetAbsoluteSteeringY() => AbsoluteSteering(NormalizedY, config.centerDeadZoneNormalizedY);

        static float AbsoluteSteering(float normalized, float deadZoneNormalized)
        {
            float centered = (normalized - 0.5f) * 2f;
            float deadZone = Mathf.Clamp01(deadZoneNormalized) * 2f;

            if (Mathf.Abs(centered) <= deadZone)
                return 0f;

            float sign = Mathf.Sign(centered);
            float magnitude = Mathf.InverseLerp(deadZone, 1f, Mathf.Abs(centered));
            return sign * magnitude;
        }

        float GetRelativeSteeringX() =>
            RelativeSteering(_pointerPosition.x - _startPosition.x, config.minDragPixelsX, config.maxDragPixelsX);

        float GetRelativeSteeringY() =>
            RelativeSteering(_pointerPosition.y - _startPosition.y, config.minDragPixelsY, config.maxDragPixelsY);

        float RelativeSteering(float delta, float minDragPixels, float maxDragPixels)
        {
            float minDrag = minDragPixels * DpiScale;
            float maxDrag = Mathf.Max(minDrag + 1f, maxDragPixels * DpiScale);

            float absDelta = Mathf.Abs(delta);

            if (absDelta <= minDrag)
                return 0f;

            float sign = Mathf.Sign(delta);
            float magnitude = Mathf.InverseLerp(minDrag, maxDrag, absDelta);
            return sign * magnitude;
        }
    }
}
