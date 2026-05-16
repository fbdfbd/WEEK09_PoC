using System.Collections.Generic;
using Project8.Application.Events;
using Project8.Domain.Model;
using UnityEngine;

namespace Project8.Application.Systems
{
    public sealed class OrderPatienceSystem : IGameTickSystem
    {
        private readonly GameRuntimeModel _game;
        private readonly IEventBus _eventBus;
        private readonly List<OrderRuntimeModel> _expiredOrders = new List<OrderRuntimeModel>();

        public OrderPatienceSystem(
            GameRuntimeModel game,
            IEventBus eventBus)
        {
            _game = game;
            _eventBus = eventBus;
        }

        public void Tick(float deltaTime)
        {
            if (!_game.IsPlaying)
            {
                return;
            }

            _expiredOrders.Clear();

            for (var i = 0; i < _game.Orders.Count; i++)
            {
                var order = _game.Orders[i];
                var remainingSeconds = Mathf.Max(
                    0f,
                    order.RemainingPatienceSeconds - deltaTime);

                order.SetRemainingPatienceSeconds(remainingSeconds);

                if (remainingSeconds <= 0f)
                {
                    _expiredOrders.Add(order);
                }
            }

            if (_game.Orders.Count > 0)
            {
                _eventBus.Publish(new OrdersChangedEvent());
            }

            for (var i = 0; i < _expiredOrders.Count; i++)
            {
                var order = _expiredOrders[i];
                _game.RemoveOrder(order);
                _eventBus.Publish(new OrderExpiredEvent(order));
            }
        }
    }
}
