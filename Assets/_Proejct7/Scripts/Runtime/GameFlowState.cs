#nullable enable

using R3;

public sealed class GameFlowState
{
    public ReactiveProperty<GamePhase> CurrentPhase { get; }
        = new(GamePhase.RequestReview);

    public ReactiveProperty<string?> SelectedRequestId { get; }
        = new(null);

    public ReactiveProperty<int> CurrentDay { get; }
        = new(1);
}