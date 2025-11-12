using System;
using System.Collections.Generic;
using UnityEngine;

namespace ErccDev.Foundation.Core.Events
{
    /// <summary>
    /// Lightweight event hub:
    ///   EventBus.StartListening("addCoins", OnAddCoins)
    ///   EventBus.Trigger("addCoins", new() { ["amount"]=1 });
    ///   EventBus.StopListening("addCoins", OnAddCoins)
    ///
    /// Lives as a single DontDestroyOnLoad GameObject.
    /// </summary>
    public class EventBus : MonoBehaviour
    {
        private static EventBus _instance;
        public static EventBus Instance => _instance;

        private readonly Dictionary<string, Action<Dictionary<string, object>>> _map
            = new(StringComparer.Ordinal);

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ---------- Static API ----------
        public static void StartListening(string eventName, Action<Dictionary<string, object>> listener)
        {
            if (_instance == null || listener == null) return;

            if (_instance._map.TryGetValue(eventName, out var chain))
                _instance._map[eventName] = chain + listener;
            else
                _instance._map[eventName] = listener;
        }

        public static void StopListening(string eventName, Action<Dictionary<string, object>> listener)
        {
            if (_instance == null || listener == null) return;

            if (_instance._map.TryGetValue(eventName, out var chain))
            {
                chain -= listener;
                if (chain == null) _instance._map.Remove(eventName);
                else _instance._map[eventName] = chain;
            }
        }

        public static void Trigger(string eventName, Dictionary<string, object> payload = null)
        {
            if (_instance == null) return;
            if (_instance._map.TryGetValue(eventName, out var chain))
                chain?.Invoke(payload);
        }
    }
}
