using System.Linq;
using App.Foundation.Data;
using App.Gameplay.Conditions;
using App.Gameplay.Definitions;
using App.Gameplay.Runtime;

namespace App.Gameplay.Ending
{
    public sealed class EndingResolver
    {
        private readonly IDataRegistry _dataRegistry;
        private readonly ConditionEvaluator _conditionEvaluator;
        private readonly GameRuntimeState _runtimeState;

        public EndingResolver(
            IDataRegistry dataRegistry,
            ConditionEvaluator conditionEvaluator,
            GameRuntimeState runtimeState)
        {
            _dataRegistry = dataRegistry;
            _conditionEvaluator = conditionEvaluator;
            _runtimeState = runtimeState;
        }

        public EndingDefinition Resolve()
        {
            var endings = _dataRegistry.GetEndings();
            var matched = endings
                .Where(ending => ending != null && !ending.IsFallback)
                .Where(ending => _conditionEvaluator.IsMet(ending.Conditions, _runtimeState))
                .OrderByDescending(ending => ending.Priority)
                .FirstOrDefault();

            return matched ?? endings.FirstOrDefault(ending => ending != null && ending.IsFallback);
        }
    }
}
