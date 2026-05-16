using System;
using System.Collections.Generic;
using Project8.Application.Events;
using Project8.Application.Services;
using Project8.Domain.Data;
using Project8.Domain.Model;

namespace Project8.Application.Systems
{
    public sealed class OrderSpawnSystem :
        IGameTickSystem,
        IEventHandler<GameStartedEvent>,
        IDisposable
    {
        private readonly GameRuntimeModel _game;
        private readonly SO_GameConfig _config;
        private readonly IOrderRepository _orderRepository;
        private readonly IRuntimeDataFactory _factory;
        private readonly IRandomService _random;
        private readonly IEventBus _eventBus;

        private float _spawnTimer;

        public OrderSpawnSystem(
            GameRuntimeModel game,
            SO_GameConfig config,
            IOrderRepository orderRepository,
            IRuntimeDataFactory factory,
            IRandomService random,
            IEventBus eventBus)
        {
            _game = game;
            _config = config;
            _orderRepository = orderRepository;
            _factory = factory;
            _random = random;
            _eventBus = eventBus;

            _eventBus.Register(this);
        }

        public void Handle(GameStartedEvent gameEvent)
        {
            _spawnTimer = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (!_game.IsPlaying)
            {
                return;
            }

            if (_game.Orders.Count >= _config.MaxActiveOrders)
            {
                return;
            }

            _spawnTimer -= deltaTime;

            if (_spawnTimer > 0f)
            {
                return;
            }

            SpawnOrder();
            ResetSpawnTimer();
        }

        public void Dispose()
        {
            _eventBus.Unregister<GameStartedEvent>(this);
        }

        private void SpawnOrder()
        {
            var orders = _orderRepository.GetAll();

            var matchingOrderCount = CountMatchingOrders(orders);

            if (matchingOrderCount == 0)
            {
                return;
            }

            var matchingIndex = _random.Range(0, matchingOrderCount);
            var definition = GetMatchingOrderAt(orders, matchingIndex);
            var order = _factory.CreateOrder(definition);

            _game.AddOrder(order);
            _eventBus.Publish(new OrderAddedEvent(order));
        }

        private int CountMatchingOrders(IReadOnlyList<SO_OrderDefinition> orders)
        {
            var count = 0;

            for (var i = 0; i < orders.Count; i++)
            {
                if (CanSpawnOrder(orders[i]))
                {
                    count++;
                }
            }

            return count;
        }

        private SO_OrderDefinition GetMatchingOrderAt(
            IReadOnlyList<SO_OrderDefinition> orders,
            int matchingIndex)
        {
            var currentIndex = 0;

            for (var i = 0; i < orders.Count; i++)
            {
                var order = orders[i];

                if (!CanSpawnOrder(order))
                {
                    continue;
                }

                if (currentIndex == matchingIndex)
                {
                    return order;
                }

                currentIndex++;
            }

            return orders[0];
        }

        private bool CanSpawnOrder(SO_OrderDefinition order)
        {
            return order != null && order.FoodType == _game.Pot.FoodType;
        }

        private void ResetSpawnTimer()
        {
            _spawnTimer = _random.Range(
                _config.OrderSpawnIntervalMin,
                _config.OrderSpawnIntervalMax);
        }
    }
}
