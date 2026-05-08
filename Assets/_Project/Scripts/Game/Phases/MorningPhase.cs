using App.Gameplay.Conditions;
using App.Gameplay.Definitions;
using App.Gameplay.Effects;
using App.Gameplay.Runtime;

namespace App.Gameplay.Phases
{
    public sealed class MorningPhase
    {
        private readonly ContentSelector _contentSelector;
        private readonly EffectProcessor _effectProcessor;
        private readonly GameRuntimeState _runtimeState;

        public MorningPhase(
            ContentSelector contentSelector,
            EffectProcessor effectProcessor,
            GameRuntimeState runtimeState)
        {
            _contentSelector = contentSelector;
            _effectProcessor = effectProcessor;
            _runtimeState = runtimeState;
        }

        public MorningIssueDefinition CurrentIssue { get; private set; }

        public MorningIssueDefinition Enter(WeekDefinition week)
        {
            CurrentIssue = _contentSelector.SelectHighestPriority(
                week?.MorningIssues,
                issue => issue.Conditions,
                issue => issue.Priority);
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
            _effectProcessor.ApplyAll(option.Effects, _runtimeState);
            return PhaseResult.Success();
        }
    }
}
