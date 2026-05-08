using App.Gameplay.Conditions;
using App.Gameplay.Definitions;
using App.Gameplay.Effects;
using App.Gameplay.Reports;
using App.Gameplay.Runtime;

namespace App.Gameplay.Phases
{
    public sealed class NoonPhase
    {
        private readonly ContentSelector _contentSelector;
        private readonly EffectProcessor _effectProcessor;
        private readonly GameRuntimeState _runtimeState;

        public NoonPhase(
            ContentSelector contentSelector,
            EffectProcessor effectProcessor,
            GameRuntimeState runtimeState)
        {
            _contentSelector = contentSelector;
            _effectProcessor = effectProcessor;
            _runtimeState = runtimeState;
        }

        public NoonActionDefinition CurrentAction { get; private set; }

        public NoonActionDefinition Enter(WeekDefinition week)
        {
            CurrentAction = _contentSelector.SelectHighestPriority(
                week?.NoonActions,
                action => action.Conditions,
                action => action.Priority);

            if (CurrentAction == null)
            {
                return null;
            }

            _effectProcessor.ApplyAll(CurrentAction.Effects, _runtimeState);
            _runtimeState.AddReport(new ReportEntry(
                _runtimeState.CurrentWeekIndex,
                CurrentAction.Title,
                CurrentAction.ReportBody));

            return CurrentAction;
        }
    }
}
