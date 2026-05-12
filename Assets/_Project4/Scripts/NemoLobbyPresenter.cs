using R3;
using UnityEngine;

namespace Project4.NurturePoc
{
    public sealed class NemoLobbyPresenter : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PocStatDefinition[] _stats;
        [SerializeField] private PocFlagDefinition[] _flags;
        [SerializeField] private PocLobbyStatusDefinition[] _lobbyStatuses;
        [SerializeField] private PocTurnIssueDefinition[] _issues;
        [SerializeField] private PocProgressDefinition[] _progresses;
        [SerializeField] private PocNightDialogueDefinition[] _nightDialogues;

        private PocFlow _flow;

        public ReactiveProperty<PocScreen> Screen { get; } = new(PocScreen.Lobby);
        public ReactiveProperty<PocLobbyViewModel> Lobby { get; } = new(PocLobbyViewModel.Empty);
        public ReactiveProperty<PocProgressViewModel> Progress { get; } = new(PocProgressViewModel.Empty);
        public ReactiveProperty<PocNightDialogueViewModel> NightDialogue { get; } = new(PocNightDialogueViewModel.Empty);
        public ReactiveProperty<PocNightFeedbackViewModel> NightFeedback { get; } = new(PocNightFeedbackViewModel.Empty);

        private void Awake()
        {
            _flow = new PocFlow(_stats, _flags, _lobbyStatuses, _issues, _progresses, _nightDialogues);
            RefreshLobby();
        }

        public void OnChoiceButtonClicked(int choiceIndex)
        {
            Progress.Value = _flow.SelectChoice(choiceIndex);
            Screen.Value = PocScreen.Progress;
        }

        public void OnAllowButtonClicked()
        {
            OnChoiceButtonClicked(0);
        }

        public void OnBlockButtonClicked()
        {
            OnChoiceButtonClicked(1);
        }

        public void OnControlButtonClicked()
        {
            OnChoiceButtonClicked(2);
        }

        public void OnCustomButtonClicked()
        {
            OnChoiceButtonClicked(3);
        }

        public void OnProgressNextClicked()
        {
            NightDialogue.Value = _flow.CompleteProgress();
            Screen.Value = PocScreen.NightDialogue;
        }

        public void OnNightChoiceButtonClicked(int choiceIndex)
        {
            NightFeedback.Value = _flow.SelectNightChoice(choiceIndex);
            Screen.Value = PocScreen.NightFeedback;
        }

        public void OnNightChoice1Clicked()
        {
            OnNightChoiceButtonClicked(0);
        }

        public void OnNightChoice2Clicked()
        {
            OnNightChoiceButtonClicked(1);
        }

        public void OnNightChoice3Clicked()
        {
            OnNightChoiceButtonClicked(2);
        }

        public void OnNightFeedbackNextClicked()
        {
            _flow.CompleteNightFeedback();

            if (!_flow.HasNextTurn())
            {
                Screen.Value = PocScreen.Finished;
                return;
            }

            RefreshLobby();
        }

        private void RefreshLobby()
        {
            Lobby.Value = _flow.BuildLobby();
            Screen.Value = PocScreen.Lobby;
        }
    }
}
