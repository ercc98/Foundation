using UnityEngine;

namespace ErccDev.Core.Gameplay
{
    public interface ICollectible
    {
        int Value { get; }
        bool TryCollect(GameObject collector);
    }
}