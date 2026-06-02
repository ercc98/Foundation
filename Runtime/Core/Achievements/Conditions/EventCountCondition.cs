using System.Collections.Generic;
using UnityEngine;
using ErccDev.Foundation.Core.Events;

namespace ErccDev.Foundation.Core.Achievements.Conditions
{
    /// <summary>
    /// Ready-to-use, game-agnostic condition: counts how many times an EventBus event fires
    /// until it reaches a target ("kill 100 enemies", "collect 50 coins"). Optionally reads
    /// an int amount from the payload instead of counting +1 per event.
    /// </summary>
    [CreateAssetMenu(menuName = "ErccDev/Achievements/Conditions/Event Count")]
    public class EventCountCondition : AchievementCondition
    {
        [Header("Trigger")]
        public string eventName;
        [Min(1)] public int target = 1;

        [Tooltip("If set, adds payload[amountKey] (int) per event instead of +1.")]
        public string amountKey;

        private int _count;

        public override void Initialize(IAchievementContext context)
        {
            _count = 0;
            EventBus.StartListening(eventName, OnEvent);
        }

        public override bool  IsCompleted() => _count >= target;
        public override float Progress01    => target > 0 ? Mathf.Clamp01(_count / (float)target) : 1f;

        public override void Cleanup() => EventBus.StopListening(eventName, OnEvent);

        private void OnEvent(Dictionary<string, object> payload)
        {
            int add = 1;
            if (!string.IsNullOrEmpty(amountKey) && payload != null &&
                payload.TryGetValue(amountKey, out var raw) && raw is int n)
                add = n;

            _count += add;
        }
    }
}
