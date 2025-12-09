using UnityEngine;

namespace ErccDev.Foundation.Core.Pooling
{
    /// <summary>
    /// Generic pool interface for renting and returning components.
    /// </summary>
    public interface IObjectPool<T> where T : Component
    {
        /// <summary>
        /// Get (rent) an instance from the pool.
        /// </summary>
        T Get();

        /// <summary>
        /// Return an instance to the pool.
        /// </summary>
        void Release(T instance);
    }
}