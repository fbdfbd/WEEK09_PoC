using System.Linq;
using App.Gameplay.Conditions;
using App.Gameplay.Definitions;
using App.Gameplay.Effects;
using App.Gameplay.Runtime;

namespace App.Gameplay.Phases
{
    public sealed class MorningPhase
    {
        private readonly ConditionEvaluator _conditionEvaluator;
        private readonly EffectProcessor _effectProcessor;
        private readonly GameRuntimeState _runtimeState;

        public MorningPhase(
            ConditionEvaluator conditionEvaluator,
            EffectProcessor effectProcessor,
            GameRuntimeState runtimeState)
        {
            _conditionEvaluator = conditionEvaluator;
            _effectProcessor = effectProcessor;
            _runtimeState = runtimeState;
        }

        public MorningIssueDefinition CurrentIssue { get; private set; }

        public MorningIssueDefinition Enter(WeekDefinition week)
        {
            CurrentIssue = week?.MorningIssues?
                .Where(issue => issue != null && _conditionEvaluator.IsMet(issue.Conditions, _runtimeState))
                .OrderByDescending(issue => issue.Priority)
                .FirstOrDefault();

            return CurrentIssue;
        }

        public PhaseResult SelectOption(int optionIndex)
        {
            if (CurrentIssue == null)
            {
                return PhaseResult.Failed("Morning issue is missing.");
            }

            if (CurrentIssue.Options == null || optionIndex < 0 || optionIndex >= CurrentIssue.Options.Length)
            {
                return PhaseResult.Failed("Morning option index is invalid.");
            }

            var option = CurrentIssue.Options[optionIndex];
            foreach (var effect in option.Effects ?? System.Array.Empty<EffectDefinition>())
            {
                _effectProcessor.Apply(effect, _runtimeState);
            }

            return PhaseResult.Success();
        }
    }
}
