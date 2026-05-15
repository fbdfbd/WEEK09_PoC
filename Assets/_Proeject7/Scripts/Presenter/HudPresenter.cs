using System;
using R3;
using VContainer.Unity;

public sealed class HudPresenter : IStartable, IDisposable
{
    private readonly HudView _view;
    private readonly GameFlowState _flowState;
    private readonly PhaseTextProvider _phaseTextProvider;

    private readonly CompositeDisposable _disposables = new();

    public HudPresenter(
        HudView view,
        GameFlowState flowState,
        PhaseTextProvider phaseTextProvider)
    {
        _view = view;
        _flowState = flowState;
        _phaseTextProvider = phaseTextProvider;
    }

    public void Start()
    {
        _flowState.CurrentDay
            .Subscribe(day => _view.SetDay(day))
            .AddTo(_disposables);

        _flowState.CurrentPhase
            .Subscribe(phase =>
            {
                var text = _phaseTextProvider.GetPhaseText(phase);
                _view.SetPhase(text);
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}