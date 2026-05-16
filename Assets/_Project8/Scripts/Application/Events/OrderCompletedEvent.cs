using Project8.Domain.Data;

namespace Project8.Application.Events
{
    public readonly struct OrderCompletedEvent : IGameEvent
    {
        public readonly ServeResult Result;

        public OrderCompletedEvent(ServeResult result)
        {
            Result = result;
        }
    }
}
