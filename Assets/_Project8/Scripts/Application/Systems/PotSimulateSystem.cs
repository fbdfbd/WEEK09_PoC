using Project8.Application.Events;
using Project8.Domain.Data;
using Project8.Domain.Model;
using Project8.Domain.Rules;

namespace Project8.Application.Systems
{
    public sealed class PotSimulateSystem : IGameTickSystem
    {
        private readonly GameRuntimeModel _game;
        private readonly SO_GameConfig _config;
        private readonly IEventBus _eventBus;

        public PotSimulateSystem(
            GameRuntimeModel game,
            SO_GameConfig config,
            IEventBus eventBus)
        {
            _game = game;
            _config = config;
            _eventBus = eventBus;
        }

        public void Tick(float deltaTime)
        {
            if (!_game.IsPlaying)
            {
                return;
            }

            PotRules.Simulate(
                _game.Pot,
                deltaTime,
                _config.ThickIncreasePerSecond,
                _config.VolumeDecreasePerSecond);

            _eventBus.Publish(new PotStateChangedEvent(_game.Pot));
        }
    }
}
