using App.Foundation.Data;
using App.Gameplay.Effects;
using App.Gameplay.Runtime;

namespace App.Gameplay.Environment
{
    public sealed class EnvironmentControlProcessor
    {
        private readonly IDataRegistry dataRegistry;
        private readonly EffectProcessor effectProcessor;
        private readonly GameRuntimeState runtimeState;

        public EnvironmentControlProcessor(
            IDataRegistry dataRegistry,
            EffectProcessor effectProcessor,
            GameRuntimeState runtimeState)
        {
            this.dataRegistry = dataRegistry;
            this.effectProcessor = effectProcessor;
            this.runtimeState = runtimeState;
        }

        public EnvironmentControlResult Apply(EnvironmentControlRequest request)
        {
            if (request == null)
            {
                return EnvironmentControlResult.Failed("Request is null.");
            }

            var definition = dataRegistry.GetEnvironmentControl(request.ControlId);
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
                effectProcessor.Apply(effect, runtimeState);
            }

            return EnvironmentControlResult.Success();
        }
    }
}
