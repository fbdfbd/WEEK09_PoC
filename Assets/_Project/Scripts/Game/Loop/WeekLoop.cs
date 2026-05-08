using App.Foundation.Data;
using App.Gameplay.Definitions;
using App.Gameplay.Ending;
using App.Gameplay.Phases;
using App.Gameplay.Runtime;

namespace App.Gameplay.Loop
{
    public sealed class WeekLoop
    {
        private readonly IDataRegistry _dataRegistry;
        private readonly GameRuntimeState _runtimeState;
        private readonly MorningPhase _morningPhase;
        private readonly NoonPhase _noonPhase;
        private readonly EveningPhase _eveningPhase;
        private readonly EndingResolver _endingResolver;

        public WeekLoop(
            IDataRegistry dataRegistry,
            GameRuntimeState runtimeState,
            MorningPhase morningPhase,
            NoonPhase noonPhase,
            EveningPhase eveningPhase,
            EndingResolver endingResolver)
        {
            _dataRegistry = dataRegistry;
            _runtimeState = runtimeState;
            _morningPhase = morningPhase;
            _noonPhase = noonPhase;
            _eveningPhase = eveningPhase;
            _endingResolver = endingResolver;
        }

        public GamePhaseType CurrentPhase { get; private set; } = GamePhaseType.Morning;
        public WeekDefinition CurrentWeek { get; private set; }
        public EndingDefinition CurrentEnding { get; private set; }

        public MorningIssueDefinition StartOrEnterMorning()
        {
            CurrentWeek = _dataRegistry.GetWeek(_runtimeState.CurrentWeekIndex);
            if (CurrentWeek == null)
            {
                return EnterEnding();
            }

            CurrentPhase = GamePhaseType.Morning;
            return _morningPhase.Enter(CurrentWeek);
        }

        public PhaseResult SelectMorningOption(int optionIndex)
        {
            return _morningPhase.SelectOption(optionIndex);
        }

        public NoonActionDefinition EnterNoon()
        {
            CurrentPhase = GamePhaseType.Noon;
            return _noonPhase.Enter(CurrentWeek);
        }

        public DialogueNodeDefinition EnterEvening()
        {
            CurrentPhase = GamePhaseType.Evening;
            return _eveningPhase.Enter(CurrentWeek);
        }

        public PhaseResult SelectEveningChoice(int choiceIndex)
        {
            return _eveningPhase.SelectChoice(choiceIndex);
        }

        public MorningIssueDefinition CompleteWeekAndEnterNextMorning()
        {
            _runtimeState.MoveToNextWeek();
            return StartOrEnterMorning();
        }

        private MorningIssueDefinition EnterEnding()
        {
            CurrentPhase = GamePhaseType.Ending;
            CurrentEnding = _endingResolver.Resolve();
            _runtimeState.MarkEndingReached();
            return null;
        }
    }
}
