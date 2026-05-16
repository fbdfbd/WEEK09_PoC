namespace Project8.Application.Events
{
    public interface IEventHandler<in TEvent>
        where TEvent : IGameEvent
    {
        void Handle(TEvent gameEvent);
    }
}
