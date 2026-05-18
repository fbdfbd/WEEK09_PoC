using System;
using VContainer.Unity;

public sealed class IntroPresenter : IStartable, IDisposable
{
    private readonly IntroView _view;
    private readonly SO_IntroSequence _sequence;
    private readonly RequestDaySystem _requestDaySystem;
    private readonly GameFlowState _flowState;

    private int _lineIndex;
    private bool _lineCompleted;
    private bool _started;

    public IntroPresenter(
        IntroView view,
        SO_IntroSequence sequence,
        RequestDaySystem requestDaySystem,
        GameFlowState flowState)
    {
        _view = view;
        _sequence = sequence;
        _requestDaySystem = requestDaySystem;
        _flowState = flowState;
    }

    public void Start()
    {
        _view.OnAdvanceClicked += HandleAdvanceClicked;
        _view.OnStartClicked += HandleStartClicked;

        _flowState.CurrentPhase.Value = GamePhase.Intro;
        _view.SetShow(true);
        _view.SetStartInteractable(false);
        _view.ClearText();

        if (_sequence.Lines == null || _sequence.Lines.Count == 0)
        {
            _view.SetStartInteractable(true);
            return;
        }

        PlayCurrentLine();
    }

    private void HandleAdvanceClicked()
    {
        if (_started)
            return;

        if (_view.IsTyping)
        {
            _view.CompleteTyping();
            return;
        }

        if (!_lineCompleted)
            return;

        _lineIndex++;
        if (_lineIndex >= _sequence.Lines.Count)
        {
            _view.SetStartInteractable(true);
            return;
        }

        PlayCurrentLine();
    }

    private void HandleStartClicked()
    {
        if (_started || _view.IsTyping)
            return;

        if (_sequence.Lines != null && _lineIndex < _sequence.Lines.Count)
            return;

        _started = true;
        _view.SetShow(false);
        _requestDaySystem.StartDay(1);
    }

    private void PlayCurrentLine()
    {
        _lineCompleted = false;
        _view.SetStartInteractable(false);
        _view.PlayLine(_sequence.Lines[_lineIndex], () =>
        {
            _lineCompleted = true;

            if (_lineIndex >= _sequence.Lines.Count - 1)
            {
                _lineIndex = _sequence.Lines.Count;
                _view.SetStartInteractable(true);
            }
        });
    }

    public void Dispose()
    {
        _view.OnAdvanceClicked -= HandleAdvanceClicked;
        _view.OnStartClicked -= HandleStartClicked;
    }
}
