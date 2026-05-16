using System;
using Project8.Application.Events;
using Project8.Domain.Model;

namespace Project8.Application.Systems
{
    public sealed class ScoreSystem :
        IEventHandler<OrderCompletedEvent>,
        IDisposable
    {
        private readonly GameRuntimeModel _game;
        private readonly IEventBus _eventBus;

        public ScoreSystem(
            GameRuntimeModel game,
            IEventBus eventBus)
        {
            _game = game;
            _eventBus = eventBus;

            _eventBus.Register(this);
        }

        public void Handle(OrderCompletedEvent gameEvent)
        {
            _game.SetScore(_game.Score + gameEvent.Result.FinalScore);
            _eventBus.Publish(new ScoreChangedEvent(_game.Score));
        }

        public void Dispose()
        {
            _eventBus.Unregister<OrderCompletedEvent>(this);
        }
    }
}
