using System;
using R3;
using VContainer.Unity;

public sealed class EndingPresenter : IStartable, IDisposable
{
    private readonly EndingView _view;
    private readonly EndingReportBuilder _reportBuilder;
    private readonly IGameSessionCommand _gameSessionCommand;
    private readonly GameFlowState _flowState;

    private readonly CompositeDisposable _disposables = new();

    public EndingPresenter(
        EndingView view,
        EndingReportBuilder reportBuilder,
        IGameSessionCommand gameSessionCommand,
        GameFlowState flowState)
    {
        _view = view;
        _reportBuilder = reportBuilder;
        _gameSessionCommand = gameSessionCommand;
        _flowState = flowState;
    }

    public void Start()
    {
        _view.OnRestartClicked += HandleRestartClicked;
        _view.OnQuitClicked += HandleQuitClicked;

        _flowState.CurrentPhase
            .Subscribe(OnPhaseChanged)
            .AddTo(_disposables);
    }

    private void OnPhaseChanged(GamePhase phase)
    {
        var isEnding = phase == GamePhase.Ending;
        _view.SetShow(isEnding);

        if (isEnding)
            _view.SetReport(_reportBuilder.Build());
    }

    private void HandleRestartClicked()
    {
        _gameSessionCommand.Restart();
    }

    private void HandleQuitClicked()
    {
        _gameSessionCommand.Quit();
    }

    public void Dispose()
    {
        _view.OnRestartClicked -= HandleRestartClicked;
        _view.OnQuitClicked -= HandleQuitClicked;
        _disposables.Dispose();
    }
}
