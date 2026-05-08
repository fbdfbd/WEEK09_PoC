using App.Foundation.Data;
using App.Gameplay.Effects;
using App.Gameplay.Runtime;

namespace App.Gameplay.Environment
{
    public sealed class EnvironmentControlProcessor
    {
        private readonly IDataRegistry _dataRegistry;
        private readonly EffectProcessor _effectProcessor;
        private readonly GameRuntimeState _runtimeState;

        public EnvironmentControlProcessor(
            IDataRegistry dataRegistry,
            EffectProcessor effectProcessor,
            GameRuntimeState runtimeState)
        {
            _dataRegistry = dataRegistry;
            _effectProcessor = effectProcessor;
            _runtimeState = runtimeState;
        }

        public EnvironmentControlResult Apply(EnvironmentControlRequest request)
        {
            if (request == null)
            {
                return EnvironmentControlResult.Failed("Request is null.");
            }

            var definition = _dataRegistry.GetEnvironmentControl(request.ControlId);
            if (definition == null)
            {
                return EnvironmentControlResult.Failed($"Unknown control id: {request.ControlId}");
            }

            if (definition.Effects == null)
            {
                return EnvironmentControlResult.Success();
            }

            foreach (var effect in definition.Effects)
            {
                _effectProcessor.Apply(effect, _runtimeState);
            }

            return EnvironmentControlResult.Success();
        }
    }
}
