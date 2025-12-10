using UnityEngine;

namespace ErccDev.Foundation.Core.Factories
{
    public interface IFactory<T> where T : Component
    {
        T Spawn(Vector3 position, Quaternion rotation, Transform parent = null);
        void Recycle(T instance);
    }
}