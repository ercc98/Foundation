using ErccDev.Foundation.Input.Interfaces;
using UnityEngine;

namespace ErccDev.Foundation.Input.Core
{
    public abstract class InputModule<TConfig> : MonoBehaviour, IInputModule where TConfig : ScriptableObject
    {
        [Header("Config (ScriptableObject)")]
        [SerializeField] protected TConfig config;

        public virtual TConfig Config
        {
            get => config;
            set
            {
                if (config == value) return;
                OnBeforeConfigChange(config, value);
                config = value;
                OnAfterConfigChange();
            }
        }

        protected float DpiScale { get; private set; } = 1f;

        protected virtual void Awake()
        {
            var dpi = Screen.dpi;
            DpiScale = dpi <= 0 ? 1f : dpi / 160f;
            ValidateOrWarn();
        }

        protected virtual void OnEnable()  => EnableModule();
        protected virtual void OnDisable() => DisableModule();

        protected abstract void EnableModule();
        protected abstract void DisableModule();

        protected virtual void ValidateOrWarn() { }
        protected virtual void OnBeforeConfigChange(TConfig oldConfig, TConfig newConfig) { }
        protected virtual void OnAfterConfigChange() { }
    }
}