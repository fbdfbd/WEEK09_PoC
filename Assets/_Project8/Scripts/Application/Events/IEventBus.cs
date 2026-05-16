namespace Project8.Application.Events
{
    public interface IEventBus
    {
        void Register<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IGameEvent;

        void Unregister<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IGameEvent;

        void Publish<TEvent>(TEvent gameEvent)
            where TEvent : IGameEvent;
    }
}
