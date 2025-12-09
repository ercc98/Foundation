using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Pooling
{
    public class ObjectPool<T> : IObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _pool = new Queue<T>();

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            for (int i = 0; i < initialSize; i++)
            {
                var obj = Object.Instantiate(_prefab, _parent);
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// Rent an object from the pool (or create a new one if empty).
        /// </summary>
        public T Get()
        {
            if (_pool.Count > 0)
                return _pool.Dequeue();

            var obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            return obj;
        }

        /// <summary>
        /// Return an object back to the pool.
        /// </summary>
        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}
