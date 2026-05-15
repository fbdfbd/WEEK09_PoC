using System;
using R3;
using VContainer.Unity;

public sealed class RequestPresenter : IStartable, IDisposable
{
    private readonly RequestView _view;
    private readonly FlowControlView _flowView;
    private readonly RequestStore _requestStore;
    private readonly RequestDecisionDraftStore _draftStore;
    private readonly GameFlowState _flowState;
    private readonly RequestSystem _requestSystem;
    private readonly RequestTextProvider _textProvider;

    private readonly CompositeDisposable _disposables = new();

    public RequestPresenter(
        RequestView view,
        FlowControlView flowView,
        RequestStore requestStore,
        RequestDecisionDraftStore draftStore,
        GameFlowState flowState,
        RequestSystem requestSystem,
        RequestTextProvider textProvider)
    {
        _view = view;
        _flowView = flowView;
        _requestStore = requestStore;
        _draftStore = draftStore;
        _flowState = flowState;
        _requestSystem = requestSystem;
        _textProvider = textProvider;
    }

    public void Start()
    {
        _view.OnReceivedClicked += HandleReceivedClicked;
        _view.OnCorrectionRequiredClicked += HandleCorrectionRequiredClicked;
        _view.OnPendingClicked += HandlePendingClicked;
        _view.OnRejectedClicked += HandleRejectedClicked;

        _flowView.OnNextClicked += HandleNextClicked;

        _flowState.SelectedRequestId
            .Subscribe(OnSelectedRequestChanged)
            .AddTo(_disposables);

        _flowState.CurrentPhase
            .Subscribe(OnPhaseChanged)
            .AddTo(_disposables);
    }

    private void OnSelectedRequestChanged(string requestId)
    {
        if (string.IsNullOrEmpty(requestId))
            return;

        var request = _requestStore.Get(requestId);

        _view.SetRequest(
            request.Title,
            request.Body,
            request.Summary
        );

        if (_flowState.CurrentPhase.Value == GamePhase.RequestReview)
            RenderDecisionState(request);
    }

    private void OnPhaseChanged(GamePhase phase)
    {
        if (phase != GamePhase.RequestReview)
            return;

        _view.SetRequestDecisionPanelShow(true);
        _view.SetAgencyAssignmentPanelShow(false);

        var requestId = _flowState.SelectedRequestId.Value;
        if (!string.IsNullOrEmpty(requestId))
            RenderDecisionState(_requestStore.Get(requestId));
    }

    private void HandleReceivedClicked()
    {
        SelectStatus(RequestStatus.Accepted);
    }

    private void HandleCorrectionRequiredClicked()
    {
        SelectStatus(RequestStatus.SupplementRequired);
    }

    private void HandlePendingClicked()
    {
        SelectStatus(RequestStatus.Deferred);
    }

    private void HandleRejectedClicked()
    {
        SelectStatus(RequestStatus.Rejected);
    }

    private void SelectStatus(RequestStatus status)
    {
        if (_flowState.CurrentPhase.Value != GamePhase.RequestReview)
            return;

        _draftStore.Set(_flowState.SelectedRequestId.Value, status);

        _view.SetInteractionTagText(_textProvider.GetStatusText(status));
        _view.SetInteractionTagShow(true);
        _view.SetAgencyTagShow(false);

        _flowView.SetNextText(_textProvider.ConfirmText);
        _flowView.SetNextInteractable(true);
    }

    private void HandleNextClicked()
    {
        if (_flowState.CurrentPhase.Value != GamePhase.RequestReview)
            return;

        if (TrySelectFirstMissingDecision())
            return;

        for (var i = 0; i < _requestStore.ActiveCount; i++)
        {
            var request = _requestStore.GetActive(i);
            if (request.Status != RequestStatus.Pending)
                continue;

            if (!_draftStore.TryGet(request.Id, out var status))
                continue;

            _requestSystem.ApplyDecision(request.Id, status);
            _draftStore.Clear(request.Id);
        }

        _requestSystem.CompleteRequestReview();
    }

    private void RenderDecisionState(RequestData request)
    {
        _view.SetAgencyTagShow(false);

        if (request.Status != RequestStatus.Pending)
        {
            _view.SetInteractionTagText(_textProvider.GetStatusText(request.Status));
            _view.SetInteractionTagShow(true);
            _view.SetButtonsInteractable(false);
            _flowView.SetNextText(_textProvider.ConfirmText);
            _flowView.SetNextInteractable(true);
            return;
        }

        if (_draftStore.TryGet(request.Id, out var draftStatus))
        {
            _view.SetInteractionTagText(_textProvider.GetStatusText(draftStatus));
            _view.SetInteractionTagShow(true);
            _view.SetButtonsInteractable(true);
            _flowView.SetNextText(_textProvider.ConfirmText);
            _flowView.SetNextInteractable(true);
            return;
        }

        ResetSelection();
    }

    private void ResetSelection()
    {
        _view.SetInteractionTagShow(false);
        _view.SetAgencyTagShow(false);
        _view.SetButtonsInteractable(true);

        _flowView.SetNextText(_textProvider.ConfirmText);
        _flowView.SetNextInteractable(true);
    }

    private bool TrySelectFirstMissingDecision()
    {
        for (var i = 0; i < _requestStore.ActiveCount; i++)
        {
            var request = _requestStore.GetActive(i);
            if (request.Status != RequestStatus.Pending)
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
        _view.OnReceivedClicked -= HandleReceivedClicked;
        _view.OnCorrectionRequiredClicked -= HandleCorrectionRequiredClicked;
        _view.OnPendingClicked -= HandlePendingClicked;
        _view.OnRejectedClicked -= HandleRejectedClicked;

        _flowView.OnNextClicked -= HandleNextClicked;

        _disposables.Dispose();
    }
}
