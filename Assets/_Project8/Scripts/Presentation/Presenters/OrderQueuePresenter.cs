using System;
using Project8.Application.Commands;
using Project8.Application.Events;
using Project8.Domain.Model;
using Project8.Presentation.Views;
using VContainer.Unity;

namespace Project8.Presentation.Presenters
{
    public sealed class OrderQueuePresenter :
        IStartable,
        IDisposable,
        IEventHandler<GameStartedEvent>,
        IEventHandler<OrderAddedEvent>,
        IEventHandler<OrdersChangedEvent>,
        IEventHandler<OrderCompletedEvent>,
        IEventHandler<OrderExpiredEvent>
    {
        private readonly IOrderQueueView _view;
        private readonly GameRuntimeModel _game;
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;

        public OrderQueuePresenter(
            IOrderQueueView view,
            GameRuntimeModel game,
            ICommandBus commandBus,
            IEventBus eventBus)
        {
            _view = view;
            _game = game;
            _commandBus = commandBus;
            _eventBus = eventBus;
        }

        public void Start()
        {
            _view.ServeOrderClicked += OnServeOrderClicked;
            _eventBus.Register<GameStartedEvent>(this);
            _eventBus.Register<OrderAddedEvent>(this);
            _eventBus.Register<OrdersChangedEvent>(this);
            _eventBus.Register<OrderCompletedEvent>(this);
            _eventBus.Register<OrderExpiredEvent>(this);
        }

        public void Handle(GameStartedEvent gameEvent)
        {
            _view.SetOrders(_game.Orders);
        }

        public void Handle(OrderAddedEvent gameEvent)
        {
            _view.SetOrders(_game.Orders);
        }

        public void Handle(OrdersChangedEvent gameEvent)
        {
            _view.SetOrders(_game.Orders);
        }

        public void Handle(OrderCompletedEvent gameEvent)
        {
            _view.SetOrders(_game.Orders);
            _view.PlayOrderCompleted(gameEvent.Result);
        }

        public void Handle(OrderExpiredEvent gameEvent)
        {
            _view.SetOrders(_game.Orders);
            _view.PlayOrderExpired(gameEvent.Order.InstanceId);
        }

        public void Dispose()
        {
            _view.ServeOrderClicked -= OnServeOrderClicked;
            _eventBus.Unregister<GameStartedEvent>(this);
            _eventBus.Unregister<OrderAddedEvent>(this);
            _eventBus.Unregister<OrdersChangedEvent>(this);
            _eventBus.Unregister<OrderCompletedEvent>(this);
            _eventBus.Unregister<OrderExpiredEvent>(this);
        }

        private void OnServeOrderClicked(string orderInstanceId)
        {
            _commandBus.Publish(new ServeOrderCommand(orderInstanceId));
        }
    }
}
