using System;
using R3;
using VContainer.Unity;

public sealed class DayFlowPresenter : IStartable, IDisposable
{
    private const int LastDay = 3;

    private readonly FlowControlView _flowView;
    private readonly RequestDaySystem _requestDaySystem;
    private readonly GameFlowState _flowState;
    private readonly RequestTextProvider _textProvider;

    private readonly CompositeDisposable _disposables = new();

    public DayFlowPresenter(
        FlowControlView flowView,
        RequestDaySystem requestDaySystem,
        GameFlowState flowState,
        RequestTextProvider textProvider)
    {
        _flowView = flowView;
        _requestDaySystem = requestDaySystem;
        _flowState = flowState;
        _textProvider = textProvider;
    }

    public void Start()
    {
        _flowView.OnNextClicked += HandleNextClicked;

        _flowState.CurrentPhase
            .Subscribe(OnPhaseChanged)
            .AddTo(_disposables);
    }

    private void OnPhaseChanged(GamePhase phase)
    {
        if (phase != GamePhase.Result)
            return;

        if (_flowState.CurrentDay.Value < LastDay)
        {
            _flowView.SetNextText(_textProvider.NextDayText);
            _flowView.SetNextInteractable(true);
            return;
        }

        _flowView.SetNextText(_textProvider.EndText);
        _flowView.SetNextInteractable(false);
    }

    private void HandleNextClicked()
    {
        if (_flowState.CurrentPhase.Value != GamePhase.Result)
            return;

        if (_flowState.CurrentDay.Value >= LastDay)
            return;

        _requestDaySystem.StartDay(_flowState.CurrentDay.Value + 1);
    }

    public void Dispose()
    {
        _flowView.OnNextClicked -= HandleNextClicked;
        _disposables.Dispose();
    }
}
