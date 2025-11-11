using UnityEngine;
using ErccDev.Core.Pooling;

namespace ErccDev.Core.Factories
{
    public abstract class Factory<T> : MonoBehaviour where T : Component
    {
        [Header("Factory Settings")]
        [SerializeField] public T prefab;
        [SerializeField] private int initialPoolSize = 10;

        // Protected so subclasses can access it
        protected ObjectPool<T> _pool;

        protected virtual void Awake()
        {
            // Create and warm the pool
            WarmPool();
        }

        public virtual void WarmPool()
        {
            if (_pool == null && prefab != null)
                _pool = new ObjectPool<T>(prefab, initialPoolSize, transform);
        }

        /// <summary>
        /// Spawns (or reuses) an instance of T at the given pose.
        /// </summary>
        public virtual T Spawn(Vector3 position, Quaternion rotation)
        {
            if (_pool == null) WarmPool();
            var instance = _pool.Get();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            return instance;
        }

        /// <summary>
        /// Returns the instance to the pool.
        /// </summary>
        public virtual void Recycle(T instance)
        {
            if (_pool == null || !instance) return;
            _pool.Release(instance);
        }
    }
}