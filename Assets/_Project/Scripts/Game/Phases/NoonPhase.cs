using System.Linq;
using App.Gameplay.Conditions;
using App.Gameplay.Definitions;
using App.Gameplay.Effects;
using App.Gameplay.Reports;
using App.Gameplay.Runtime;

namespace App.Gameplay.Phases
{
    public sealed class NoonPhase
    {
        private readonly ConditionEvaluator _conditionEvaluator;
        private readonly EffectProcessor _effectProcessor;
        private readonly GameRuntimeState _runtimeState;

        public NoonPhase(
            ConditionEvaluator conditionEvaluator,
            EffectProcessor effectProcessor,
            GameRuntimeState runtimeState)
        {
            _conditionEvaluator = conditionEvaluator;
            _effectProcessor = effectProcessor;
            _runtimeState = runtimeState;
        }

        public NoonActionDefinition CurrentAction { get; private set; }

        public NoonActionDefinition Enter(WeekDefinition week)
        {
            CurrentAction = week?.NoonActions?
                .Where(action => action != null && _conditionEvaluator.IsMet(action.Conditions, _runtimeState))
                .OrderByDescending(action => action.Priority)
                .FirstOrDefault();

            if (CurrentAction == null)
            {
                return null;
            }

            foreach (var effect in CurrentAction.Effects ?? System.Array.Empty<EffectDefinition>())
            {
                _effectProcessor.Apply(effect, _runtimeState);
            }

            _runtimeState.AddReport(new ReportEntry(
                _runtimeState.CurrentWeekIndex,
                CurrentAction.Title,
                CurrentAction.ReportBody));

            return CurrentAction;
        }
    }
}
