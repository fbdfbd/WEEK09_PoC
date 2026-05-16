using System;
using System.Collections.Generic;

namespace Project8.Application.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();

        public void Register<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IGameEvent
        {
            var eventType = typeof(TEvent);

            if (!_handlers.TryGetValue(eventType, out var handlers))
            {
                handlers = new List<object>();
                _handlers.Add(eventType, handlers);
            }

            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        public void Unregister<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IGameEvent
        {
            var eventType = typeof(TEvent);

            if (!_handlers.TryGetValue(eventType, out var handlers))
            {
                return;
            }

            handlers.Remove(handler);
        }

        public void Publish<TEvent>(TEvent gameEvent)
            where TEvent : IGameEvent
        {
            var eventType = typeof(TEvent);

            if (!_handlers.TryGetValue(eventType, out var handlers))
            {
                return;
            }

            for (var i = 0; i < handlers.Count; i++)
            {
                var handler = (IEventHandler<TEvent>)handlers[i];
                handler.Handle(gameEvent);
            }
        }
    }
}
