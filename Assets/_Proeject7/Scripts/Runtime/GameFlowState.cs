#nullable enable

using R3;

public sealed class GameFlowState
{
    public ReactiveProperty<GamePhase> CurrentPhase { get; }
        = new(GamePhase.Intro);

    public ReactiveProperty<string?> SelectedRequestId { get; }
        = new(null);

    public ReactiveProperty<int> SelectedRequestIndex { get; }
        = new(0);

    public ReactiveProperty<int> CurrentDay { get; }
        = new(1);
}
