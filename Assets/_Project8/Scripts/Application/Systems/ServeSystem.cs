using System;
using Project8.Application.Commands;
using Project8.Application.Events;
using Project8.Domain.Data;
using Project8.Domain.Model;
using Project8.Domain.Rules;

namespace Project8.Application.Systems
{
    public sealed class ServeSystem :
        ICommandHandler<ServeOrderCommand>,
        IDisposable
    {
        private readonly GameRuntimeModel _game;
        private readonly SO_GameConfig _config;
        private readonly TasteScoreCalculator _scoreCalculator;
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;

        public ServeSystem(
            GameRuntimeModel game,
            SO_GameConfig config,
            TasteScoreCalculator scoreCalculator,
            ICommandBus commandBus,
            IEventBus eventBus)
        {
            _game = game;
            _config = config;
            _scoreCalculator = scoreCalculator;
            _commandBus = commandBus;
            _eventBus = eventBus;

            _commandBus.Register(this);
        }

        public void Handle(ServeOrderCommand command)
        {
            if (!_game.IsPlaying)
            {
                return;
            }

            if (!PotRules.CanServe(_game.Pot, _config.MinimumServingVolume))
            {
                _eventBus.Publish(new InvalidActionEvent("냄비의 양이 부족합니다."));
                return;
            }

            if (!_game.TryGetOrder(command.OrderInstanceId, out var order))
            {
                _eventBus.Publish(new InvalidActionEvent("주문을 찾을 수 없습니다."));
                return;
            }

            var result = _scoreCalculator.Calculate(
                _game.Pot,
                order,
                _config.BurnPenaltyThreshold);

            if (!result.IsSuccess)
            {
                _eventBus.Publish(new InvalidActionEvent("지금 냄비 상태로는 이 주문을 낼 수 없습니다."));
                return;
            }

            PotRules.ConsumeServing(_game.Pot, _config.ServingVolumeCost);
            _game.RemoveOrder(order);

            _eventBus.Publish(new PotStateChangedEvent(_game.Pot));
            _eventBus.Publish(new OrderCompletedEvent(result));
        }

        public void Dispose()
        {
            _commandBus.Unregister<ServeOrderCommand>(this);
        }
    }
}
