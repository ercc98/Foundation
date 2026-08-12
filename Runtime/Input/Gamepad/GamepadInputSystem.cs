using System;
using System.Collections.Generic;
using ErccDev.Foundation.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ErccDev.Foundation.Input.Gamepad
{
    /// <summary>
    /// Config-driven gamepad reader built on the New Input System. Wires the action references
    /// on its <see cref="GamepadInputConfig"/> and exposes sticks, buttons, triggers and D-pad
    /// through focused interfaces so consumers depend only on the input they use.
    /// </summary>
    public class GamepadInputSystem :
        InputModule<GamepadInputConfig>,
        IGamepadSticksInput, IGamepadButtonsInput, IGamepadTriggersInput, IGamepadDpadInput
    {
        // ---------- Events ----------

        public event Action<Vector2> MoveChanged;
        public event Action<Vector2> LookChanged;

        public event Action<GamepadButton> ButtonPressed;
        public event Action<GamepadButton> ButtonReleased;

        public event Action<float> LeftTriggerChanged;
        public event Action<float> RightTriggerChanged;

        public event Action DpadUp;
        public event Action DpadDown;
        public event Action DpadLeft;
        public event Action DpadRight;
        public event Action DpadReturnedToCenterX;
        public event Action DpadReturnedToCenterY;

        // ---------- Sticks ----------

        public Vector2 Move => ApplyRadialDeadZone(_rawMove);
        public Vector2 Look => ApplyRadialDeadZone(_rawLook);

        public bool IsConnected => UnityEngine.InputSystem.Gamepad.current != null;

        // ---------- Triggers ----------

        public float LeftTrigger  => ApplyTriggerThreshold(_rawLeftTrigger);
        public float RightTrigger => ApplyTriggerThreshold(_rawRightTrigger);

        public bool IsLeftTriggerHeld  => LeftTrigger  > 0f;
        public bool IsRightTriggerHeld => RightTrigger > 0f;

        // ---------- D-pad ----------

        public bool IsLeft  => DpadAxis(_rawDpad.x) < 0;
        public bool IsRight => DpadAxis(_rawDpad.x) > 0;
        public bool IsUp    => DpadAxis(_rawDpad.y) > 0;
        public bool IsDown  => DpadAxis(_rawDpad.y) < 0;

        Vector2 _rawMove;
        Vector2 _rawLook;
        Vector2 _rawDpad;
        float _rawLeftTrigger;
        float _rawRightTrigger;

        readonly HashSet<GamepadButton> _pressed = new();
        readonly List<Action> _teardown = new();

        AxisState _dpadX = AxisState.Center;
        AxisState _dpadY = AxisState.Center;

        enum AxisState
        {
            Center,
            Negative,
            Positive
        }

        // ---------- Module lifecycle ----------

        public override void EnableModule()
        {
            if (config == null)
                return;

            Bind(config.moveAction, performed: OnMovePerformed, canceled: OnMoveCanceled);
            Bind(config.lookAction, performed: OnLookPerformed, canceled: OnLookCanceled);

            BindButton(config.buttonSouth,   GamepadButton.South);
            BindButton(config.buttonEast,    GamepadButton.East);
            BindButton(config.buttonWest,    GamepadButton.West);
            BindButton(config.buttonNorth,   GamepadButton.North);
            BindButton(config.leftShoulder,  GamepadButton.LeftShoulder);
            BindButton(config.rightShoulder, GamepadButton.RightShoulder);

            Bind(config.leftTrigger,  performed: OnLeftTrigger,  canceled: OnLeftTrigger);
            Bind(config.rightTrigger, performed: OnRightTrigger, canceled: OnRightTrigger);

            Bind(config.dpad, performed: OnDpad, canceled: OnDpad);
        }

        public override void DisableModule()
        {
            for (int i = _teardown.Count - 1; i >= 0; i--)
                _teardown[i]?.Invoke();

            _teardown.Clear();
        }

        protected override void ValidateOrWarn()
        {
#if UNITY_EDITOR
            if (config == null)
                Debug.LogWarning($"[{nameof(GamepadInputSystem)}] Missing config ScriptableObject.", this);
#endif
        }

        protected override void OnAfterConfigChange()
        {
            if (!isActiveAndEnabled)
                return;

            DisableModule();
            EnableModule();
        }

        // ---------- Wiring helpers ----------

        void Bind(InputActionReference reference,
                  Action<InputAction.CallbackContext> started  = null,
                  Action<InputAction.CallbackContext> performed = null,
                  Action<InputAction.CallbackContext> canceled  = null)
        {
            if (reference?.action == null)
                return;

            var action = reference.action;

            if (started != null)
            {
                action.started += started;
                _teardown.Add(() => action.started -= started);
            }

            if (performed != null)
            {
                action.performed += performed;
                _teardown.Add(() => action.performed -= performed);
            }

            if (canceled != null)
            {
                action.canceled += canceled;
                _teardown.Add(() => action.canceled -= canceled);
            }

            action.Enable();
            _teardown.Add(action.Disable);
        }

        void BindButton(InputActionReference reference, GamepadButton button)
        {
            Bind(reference,
                started:  _ => SetButton(button, true),
                canceled: _ => SetButton(button, false));
        }

        // ---------- Callbacks ----------

        void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _rawMove = ctx.ReadValue<Vector2>();
            MoveChanged?.Invoke(Move);
        }

        void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _rawMove = Vector2.zero;
            MoveChanged?.Invoke(Move);
        }

        void OnLookPerformed(InputAction.CallbackContext ctx)
        {
            _rawLook = ctx.ReadValue<Vector2>();
            LookChanged?.Invoke(Look);
        }

        void OnLookCanceled(InputAction.CallbackContext ctx)
        {
            _rawLook = Vector2.zero;
            LookChanged?.Invoke(Look);
        }

        void OnLeftTrigger(InputAction.CallbackContext ctx)
        {
            _rawLeftTrigger = ctx.ReadValue<float>();
            LeftTriggerChanged?.Invoke(LeftTrigger);
        }

        void OnRightTrigger(InputAction.CallbackContext ctx)
        {
            _rawRightTrigger = ctx.ReadValue<float>();
            RightTriggerChanged?.Invoke(RightTrigger);
        }

        void OnDpad(InputAction.CallbackContext ctx)
        {
            _rawDpad = ctx.ReadValue<Vector2>();
            EvaluateDpadX();
            EvaluateDpadY();
        }

        void SetButton(GamepadButton button, bool pressed)
        {
            if (pressed)
            {
                if (_pressed.Add(button))
                    ButtonPressed?.Invoke(button);
            }
            else
            {
                if (_pressed.Remove(button))
                    ButtonReleased?.Invoke(button);
            }
        }

        public bool IsPressed(GamepadButton button) => _pressed.Contains(button);

        // ---------- D-pad evaluation ----------

        void EvaluateDpadX()
        {
            var newState = StateFromAxis(_rawDpad.x);

            if (newState == _dpadX)
                return;

            _dpadX = newState;

            switch (_dpadX)
            {
                case AxisState.Negative: DpadLeft?.Invoke();  break;
                case AxisState.Positive: DpadRight?.Invoke(); break;
                default: DpadReturnedToCenterX?.Invoke();     break;
            }
        }

        void EvaluateDpadY()
        {
            var newState = StateFromAxis(_rawDpad.y);

            if (newState == _dpadY)
                return;

            _dpadY = newState;

            switch (_dpadY)
            {
                case AxisState.Positive: DpadUp?.Invoke();   break;
                case AxisState.Negative: DpadDown?.Invoke(); break;
                default: DpadReturnedToCenterY?.Invoke();    break;
            }
        }

        AxisState StateFromAxis(float value)
        {
            int axis = DpadAxis(value);
            if (axis < 0) return AxisState.Negative;
            if (axis > 0) return AxisState.Positive;
            return AxisState.Center;
        }

        int DpadAxis(float value)
        {
            float threshold = config != null ? config.dpadThreshold : 0.5f;
            if (value >  threshold) return 1;
            if (value < -threshold) return -1;
            return 0;
        }

        // ---------- Shaping ----------

        Vector2 ApplyRadialDeadZone(Vector2 raw)
        {
            if (config == null)
                return Vector2.zero;

            float deadZone = Mathf.Clamp01(config.stickDeadZone);
            float magnitude = raw.magnitude;

            if (magnitude <= deadZone)
                return Vector2.zero;

            float scaled = Mathf.InverseLerp(deadZone, 1f, magnitude);
            return raw.normalized * Mathf.Clamp01(scaled);
        }

        float ApplyTriggerThreshold(float value)
        {
            if (config == null)
                return 0f;

            return value < config.triggerThreshold ? 0f : Mathf.Clamp01(value);
        }
    }
}
