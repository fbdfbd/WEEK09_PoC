using Project8.Application.Commands;
using Project8.Application.Systems;
using VContainer.Unity;

namespace Project8.Infrastructure.Installers
{
    public sealed class GameLoopEntryPoint : IStartable, ITickable
    {
        private readonly ICommandBus _commandBus;
        private readonly IGameTickSystem[] _tickSystems;

        public GameLoopEntryPoint(
            ICommandBus commandBus,
            GameFlowSystem gameFlowSystem,
            PotSimulateSystem potSimulateSystem,
            OrderSpawnSystem orderSpawnSystem,
            OrderPatienceSystem orderPatienceSystem,
            IngredientApplySystem ingredientApplySystem,
            ServeSystem serveSystem,
            ScoreSystem scoreSystem)
        {
            _commandBus = commandBus;

            _tickSystems = new IGameTickSystem[]
            {
                gameFlowSystem,
                potSimulateSystem,
                orderSpawnSystem,
                orderPatienceSystem
            };
        }

        public void Start()
        {
            _commandBus.Publish(new StartGameCommand());
        }

        public void Tick()
        {
            var deltaTime = UnityEngine.Time.deltaTime;

            for (var i = 0; i < _tickSystems.Length; i++)
            {
                _tickSystems[i].Tick(deltaTime);
            }
        }
    }
}
