using System;
using Project8.Application.Commands;
using Project8.Application.Events;
using Project8.Application.Services;
using Project8.Domain.Data;
using Project8.Domain.Model;
using UnityEngine;

namespace Project8.Application.Systems
{
    public sealed class GameFlowSystem :
        IGameTickSystem,
        ICommandHandler<StartGameCommand>,
        ICommandHandler<RestartGameCommand>,
        IDisposable
    {
        private readonly GameRuntimeModel _game;
        private readonly SO_GameConfig _config;
        private readonly IRuntimeDataFactory _factory;
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;

        public GameFlowSystem(
            GameRuntimeModel game,
            SO_GameConfig config,
            IRuntimeDataFactory factory,
            ICommandBus commandBus,
            IEventBus eventBus)
        {
            _game = game;
            _config = config;
            _factory = factory;
            _commandBus = commandBus;
            _eventBus = eventBus;

            _commandBus.Register<StartGameCommand>(this);
            _commandBus.Register<RestartGameCommand>(this);
        }

        public void Handle(StartGameCommand command)
        {
            StartGame();
        }

        public void Handle(RestartGameCommand command)
        {
            StartGame();
        }

        public void Tick(float deltaTime)
        {
            if (!_game.IsPlaying)
            {
                return;
            }

            var remainingSeconds = Mathf.Max(
                0f,
                _game.RemainingSeconds - deltaTime);

            _game.SetRemainingSeconds(remainingSeconds);
            _eventBus.Publish(new TimeChangedEvent(remainingSeconds));

            if (remainingSeconds <= 0f)
            {
                EndGame();
            }
        }

        public void Dispose()
        {
            _commandBus.Unregister<StartGameCommand>(this);
            _commandBus.Unregister<RestartGameCommand>(this);
        }

        private void StartGame()
        {
            _game.SetPot(_factory.CreatePot(_config));
            _game.ClearOrders();
            _game.SetScore(0);
            _game.SetRemainingSeconds(_config.GameSeconds);
            _game.SetPlaying(true);

            _eventBus.Publish(new GameStartedEvent());
            _eventBus.Publish(new PotStateChangedEvent(_game.Pot));
            _eventBus.Publish(new ScoreChangedEvent(_game.Score));
            _eventBus.Publish(new TimeChangedEvent(_game.RemainingSeconds));
        }

        private void EndGame()
        {
            _game.SetPlaying(false);
            _eventBus.Publish(new GameEndedEvent(_game.Score));
        }
    }
}
