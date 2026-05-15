using System;
using R3;
using VContainer.Unity;

public sealed class AgencyAssignmentPresenter : IStartable, IDisposable
{
    private readonly RequestView _view;
    private readonly FlowControlView _flowView;
    private readonly RequestStore _requestStore;
    private readonly AgencyAssignmentDraftStore _draftStore;
    private readonly AgencyAssignmentSystem _agencyAssignmentSystem;
    private readonly AgencyStore _agencyStore;
    private readonly GameFlowState _flowState;
    private readonly RequestTextProvider _textProvider;

    private readonly CompositeDisposable _disposables = new();

    public AgencyAssignmentPresenter(
        RequestView view,
        FlowControlView flowView,
        RequestStore requestStore,
        AgencyAssignmentDraftStore draftStore,
        AgencyAssignmentSystem agencyAssignmentSystem,
        AgencyStore agencyStore,
        GameFlowState flowState,
        RequestTextProvider textProvider)
    {
        _view = view;
        _flowView = flowView;
        _requestStore = requestStore;
        _draftStore = draftStore;
        _agencyAssignmentSystem = agencyAssignmentSystem;
        _agencyStore = agencyStore;
        _flowState = flowState;
        _textProvider = textProvider;
    }

    public void Start()
    {
        _view.OnAgencyClicked += HandleAgencyClicked;
        _flowView.OnNextClicked += HandleNextClicked;

        _flowState.CurrentPhase
            .Subscribe(OnPhaseChanged)
            .AddTo(_disposables);

        _flowState.SelectedRequestId
            .Subscribe(_ => RefreshAgencyAssignmentView())
            .AddTo(_disposables);
    }

    private void OnPhaseChanged(GamePhase phase)
    {
        if (phase == GamePhase.AgencyAssignment)
        {
            RefreshAgencyAssignmentView();
            return;
        }

        if (phase == GamePhase.Result)
        {
            _view.SetRequestDecisionPanelShow(false);
            _view.SetAgencyAssignmentPanelShow(false);
            _view.SetInteractionTagShow(false);
            _view.SetAgencyTagShow(false);
        }
    }

    private void RefreshAgencyAssignmentView()
    {
        if (_flowState.CurrentPhase.Value != GamePhase.AgencyAssignment)
            return;

        _view.SetRequestDecisionPanelShow(false);
        _view.SetAgencyAssignmentPanelShow(true);
        _flowView.SetNextText(_textProvider.ConfirmText);
        _flowView.SetNextInteractable(true);

        var requestId = _flowState.SelectedRequestId.Value;
        if (string.IsNullOrEmpty(requestId))
            return;

        var request = _requestStore.Get(requestId);
        _view.SetInteractionTagText(_textProvider.GetStatusText(request.Status));
        _view.SetInteractionTagShow(true);

        if (!_draftStore.TryGet(requestId, out var agencyId))
        {
            _view.SetAgencyTagShow(false);
            return;
        }

        var agency = _agencyStore.Get(agencyId);
        _view.SetAgencyTagText(_textProvider.GetAgencyAssignmentText(agency));
        _view.SetAgencyTagShow(true);
    }

    private void HandleAgencyClicked(string agencyId)
    {
        if (_flowState.CurrentPhase.Value != GamePhase.AgencyAssignment)
            return;

        var requestId = _flowState.SelectedRequestId.Value;
        if (string.IsNullOrEmpty(requestId))
            return;

        _draftStore.Set(requestId, agencyId);

        var agency = _agencyStore.Get(agencyId);
        _view.SetAgencyTagText(_textProvider.GetAgencyAssignmentText(agency));
        _view.SetAgencyTagShow(true);
    }

    private void HandleNextClicked()
    {
        if (_flowState.CurrentPhase.Value != GamePhase.AgencyAssignment)
            return;

        if (TrySelectFirstMissingAgency())
            return;

        for (var i = 0; i < _requestStore.AssignmentTargetCount; i++)
        {
            var request = _requestStore.GetAssignmentTarget(i);
            if (_requestStore.IsAssignmentCompleted(request.Id))
                continue;

            if (!_draftStore.TryGet(request.Id, out var agencyId))
                continue;

            _agencyAssignmentSystem.AssignAgency(request.Id, agencyId);
            _draftStore.Clear(request.Id);
        }

        _agencyAssignmentSystem.CompleteAgencyAssignment();
    }

    private bool TrySelectFirstMissingAgency()
    {
        for (var i = 0; i < _requestStore.AssignmentTargetCount; i++)
        {
            var request = _requestStore.GetAssignmentTarget(i);
            if (_requestStore.IsAssignmentCompleted(request.Id))
                continue;

            if (_draftStore.Has(request.Id))
                continue;

            _flowState.SelectedRequestIndex.Value = i;
            _flowState.SelectedRequestId.Value = request.Id;
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        _view.OnAgencyClicked -= HandleAgencyClicked;
        _flowView.OnNextClicked -= HandleNextClicked;
        _disposables.Dispose();
    }
}
