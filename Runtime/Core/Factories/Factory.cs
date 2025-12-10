using UnityEngine;
using ErccDev.Foundation.Core.Pooling;

namespace ErccDev.Foundation.Core.Factories
{
    public abstract class Factory<T> : MonoBehaviour, IFactory<T> where T : Component
    {
        [Header("Factory Settings")]
        [SerializeField] public T prefab;
        [SerializeField] private int initialPoolSize = 10;

        protected IObjectPool<T> _pool;

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
        /// Optional external configuration (DI, tests, etc).
        /// Call this BEFORE Awake/Spawn if you want a custom pool.
        /// </summary>
        public void SetPool(IObjectPool<T> pool)
        {
            _pool = pool;
        }

        public virtual T Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_pool == null) WarmPool();
            var instance = _pool.Get();
            instance.transform.SetPositionAndRotation(position, rotation);
            if (parent != null) instance.transform.SetParent(parent);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public virtual void Recycle(T instance)
        {
            if (_pool == null || !instance) return;
            _pool.Release(instance);
        }
    }
}