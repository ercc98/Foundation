using System;
using System.Collections.Generic;

namespace ErccDev.Foundation.Core.Tutorial
{
    public sealed class TutorialContext : ITutorialContext
    {
        private readonly Dictionary<Type, object> _services = new();

        public TutorialContext Add<T>(T service) where T : class
        {
            if (service == null) return this;
            _services[typeof(T)] = service;
            return this;
        }

        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj) && obj is T cast)
            {
                service = cast;
                return true;
            }

            service = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            return TryGet<T>(out T service) ? service : null;
        }

        public void Remove<T>() where T : class
        {
            _services.Remove(typeof(T));
        }

        public void Clear()
        {
            _services.Clear();
        }
    }
}