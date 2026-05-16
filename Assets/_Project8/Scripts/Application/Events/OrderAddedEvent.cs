using Project8.Domain.Model;

namespace Project8.Application.Events
{
    public readonly struct OrderAddedEvent : IGameEvent
    {
        public readonly OrderRuntimeModel Order;

        public OrderAddedEvent(OrderRuntimeModel order)
        {
            Order = order;
        }
    }

    public readonly struct OrdersChangedEvent : IGameEvent
    {
    }
}
