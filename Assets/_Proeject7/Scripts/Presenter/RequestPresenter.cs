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
        _view.OnRejectReasonClicked += HandleRejectReasonClicked;

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
        var requestId = _flowState.SelectedRequestId.Value;
        if (string.IsNullOrEmpty(requestId) || !_requestStore.Get(requestId).CanDefer)
            return;

        SelectStatus(RequestStatus.Deferred);
    }

    private void HandleRejectedClicked()
    {
        if (_flowState.CurrentPhase.Value != GamePhase.RequestReview)
            return;

        _view.SetRejectReasonPanelShow(true);
        _view.SetInteractionTagText(_textProvider.GetStatusText(RequestStatus.Rejected));
        _view.SetInteractionTagShow(true);
        _view.SetAgencyTagShow(false);

        _flowView.SetNextText(_textProvider.ConfirmText);
        _flowView.SetNextInteractable(false);
    }

    private void HandleRejectReasonClicked(RejectReason reason)
    {
        if (_flowState.CurrentPhase.Value != GamePhase.RequestReview)
            return;

        _draftStore.SetRejected(_flowState.SelectedRequestId.Value, reason);

        _view.SetInteractionTagText($"{_textProvider.GetStatusText(RequestStatus.Rejected)} / {_textProvider.GetRejectReasonText(reason)}");
        _view.SetInteractionTagShow(true);
        _view.SetAgencyTagShow(false);

        _flowView.SetNextText(_textProvider.ConfirmText);
        _flowView.SetNextInteractable(true);
    }

    private void SelectStatus(RequestStatus status)
    {
        if (_flowState.CurrentPhase.Value != GamePhase.RequestReview)
            return;

        _view.SetRejectReasonPanelShow(false);
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

            if (!_draftStore.TryGet(request.Id, out var draft))
                continue;

            _requestSystem.ApplyDecision(request.Id, draft);
            _draftStore.Clear(request.Id);
        }

        _requestSystem.CompleteRequestReview();
    }

    private void RenderDecisionState(RequestData request)
    {
        _view.SetAgencyTagShow(false);
        _view.SetRejectReasonPanelShow(false);
        _view.SetDeadlineText(_textProvider.GetDeadlineText(request));

        if (request.Status != RequestStatus.Pending)
        {
            _view.SetInteractionTagText(_textProvider.GetStatusText(request.Status));
            _view.SetInteractionTagShow(true);
            _view.SetButtonsInteractable(false);
            _flowView.SetNextText(_textProvider.ConfirmText);
            _flowView.SetNextInteractable(true);
            return;
        }

        if (_draftStore.TryGet(request.Id, out var draft))
        {
            var text = draft.Status == RequestStatus.Rejected && draft.HasRejectReason
                ? $"{_textProvider.GetStatusText(draft.Status)} / {_textProvider.GetRejectReasonText(draft.RejectReason)}"
                : _textProvider.GetStatusText(draft.Status);

            _view.SetInteractionTagText(text);
            _view.SetInteractionTagShow(true);
            _view.SetButtonsInteractable(true);
            _view.SetPendingButtonInteractable(request.CanDefer);
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
        _view.SetRejectReasonPanelShow(false);
        _view.SetButtonsInteractable(true);

        var requestId = _flowState.SelectedRequestId.Value;
        if (!string.IsNullOrEmpty(requestId))
            _view.SetPendingButtonInteractable(_requestStore.Get(requestId).CanDefer);

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
        _view.OnRejectReasonClicked -= HandleRejectReasonClicked;

        _flowView.OnNextClicked -= HandleNextClicked;

        _disposables.Dispose();
    }
}
