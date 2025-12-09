using UnityEngine;

namespace ErccDev.Foundation.Input.Core
{
    public abstract class InputModule<T> : MonoBehaviour, IInputModule<T> where T : ScriptableObject
    {
        [Header("Config (ScriptableObject)")]
        [SerializeField] protected T config;

        public virtual T Config
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

        public abstract void EnableModule();
        public abstract void DisableModule();

        protected virtual void ValidateOrWarn() { }
        protected virtual void OnBeforeConfigChange(T oldConfig, T newConfig) { }
        protected virtual void OnAfterConfigChange() { }

    }
}