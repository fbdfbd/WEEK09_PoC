using Project8.Domain.Model;

namespace Project8.Application.Events
{
    public readonly struct OrderExpiredEvent : IGameEvent
    {
        public readonly OrderRuntimeModel Order;

        public OrderExpiredEvent(OrderRuntimeModel order)
        {
            Order = order;
        }
    }
}
