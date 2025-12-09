using UnityEngine;

namespace ErccDev.Foundation.Input.Core
{
    /// <summary>
    /// Abstraction for a configurable input module that uses ScriptableObject-based settings.
    /// </summary>
    public interface IInputModule<T> where T : ScriptableObject
    {
        /// <summary>
        /// Configuration asset used by this input module.
        /// Setting this triggers config-change logic in the implementation.
        /// </summary>
        T Config { get; set; }

        /// <summary>
        /// Called when the module is enabled (e.g., input should start listening or subscribing).
        /// </summary>
        void EnableModule();

        /// <summary>
        /// Called when the module is disabled (e.g., input should stop listening or subscribing).
        /// </summary>
        void DisableModule();
    }
}