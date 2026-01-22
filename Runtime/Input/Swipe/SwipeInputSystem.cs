using System;
using ErccDev.Foundation.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ErccDev.Foundation.Input.Swipe
{
    public class SwipeInputSystem : InputModule<SwipeInputConfig>, ISwipeInput, ITouchInput
    {
        public event Action SwipeLeft;
        public event Action SwipeRight;
        public event Action SwipeUp;
        public event Action SwipeDown;
        public event Action Tap;
        public event Action StartTap;
        public event Action EndTap;

        Vector2 _startPos;
        double  _startTime;
        bool    _isPressing;
        bool    _moved;

        public override void EnableModule()
        {
            if (config?.pointerPress != null)
            {
                var press = config.pointerPress.action;
                press.started  += OnPressStarted;
                press.canceled += OnPressCanceled;
                press.Enable();
            }

            if (config?.pointerPosition != null)
                config.pointerPosition.action.Enable();
        }

        public override void DisableModule()
        {
            if (config?.pointerPress != null)
            {
                var press = config.pointerPress.action;
                press.started  -= OnPressStarted;
                press.canceled -= OnPressCanceled;
                press.Disable();
            }

            if (config?.pointerPosition != null)
                config.pointerPosition.action.Disable();
        }

        protected override void ValidateOrWarn()
        {
#if UNITY_EDITOR
            if (config == null)
                Debug.LogWarning($"[{nameof(SwipeInputSystem)}] Missing config ScriptableObject.", this);
#endif
        }

        protected override void OnAfterConfigChange()
        {
            DisableModule();
            EnableModule();
        }

        void OnPressStarted(InputAction.CallbackContext ctx)
        {
            _isPressing = true;
            _moved      = false;
            _startTime  = ctx.time;
            _startPos   = ReadPointer();
            
            StartTap?.Invoke();
        }

        void OnPressCanceled(InputAction.CallbackContext ctx)
        {
            if (!_isPressing) return;
            _isPressing = false;

            Vector2 endPos = ReadPointer();
            Vector2 delta  = endPos - _startPos;

            float minSwipe = config.minSwipePixels * DpiScale;
            float tapMax   = config.tapMaxPixels   * DpiScale;
            double heldFor = ctx.time - _startTime;

            if (delta.magnitude > tapMax)
                _moved = true;

            if (!_moved && heldFor <= config.tapMaxTime)
            {
                EndTap?.Invoke();
                Tap?.Invoke();
                return;
            }

            if (delta.magnitude >= minSwipe)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x > 0f) SwipeRight?.Invoke();
                    else              SwipeLeft?.Invoke();
                }
                else
                {
                    if (delta.y > 0f) SwipeUp?.Invoke();
                    else              SwipeDown?.Invoke();
                }
            }
        }

        Vector2 ReadPointer()
        {
            return config?.pointerPosition != null
                ? config.pointerPosition.action.ReadValue<Vector2>()
                : Vector2.zero;
        }
    }
}