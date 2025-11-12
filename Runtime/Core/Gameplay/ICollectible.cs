using UnityEngine;

namespace ErccDev.Foundation.Core.Gameplay
{
    public interface ICollectible
    {
        int Value { get; }
        bool TryCollect(GameObject collector);
    }
}