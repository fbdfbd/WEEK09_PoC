using System;
using R3;
using VContainer.Unity;

public sealed class RequestPresenter : IStartable, IDisposable
{
    private readonly RequestView _view;
    private readonly FlowControlView _flowView;
    private readonly RequestStore _requestStore;
    private readonly GameFlowState _flowState;
    private readonly RequestSystem _requestSystem;

    private readonly CompositeDisposable _disposables = new();

    private RequestStatus? _selectedStatus;

    public RequestPresenter(
        RequestView view,
        FlowControlView flowView,
        RequestStore requestStore,
        GameFlowState flowState,
        RequestSystem requestSystem)
    {
        _view = view;
        _flowView = flowView;
        _requestStore = requestStore;
        _flowState = flowState;
        _requestSystem = requestSystem;
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

        ResetSelection();
    }

    private void HandleReceivedClicked()
    {
        SelectStatus(
            RequestStatus.Accepted,
            "접수"
        );
    }

    private void HandleCorrectionRequiredClicked()
    {
        SelectStatus(
            RequestStatus.SupplementRequired,
            "보완요구"
        );
    }

    private void HandlePendingClicked()
    {
        SelectStatus(
            RequestStatus.Deferred,
            "심의 보류"
        );
    }

    private void HandleRejectedClicked()
    {
        SelectStatus(
            RequestStatus.Rejected,
            "기각"
        );
    }

    private void SelectStatus(RequestStatus status, string memoText)
    {
        _selectedStatus = status;

        _view.SetInteractionTagText(memoText);
        _view.SetInteractionTagShow(true);

        // 이 단계에서는 기관 배정 태그 안 씀
        _view.SetAgencyTagShow(false);

        _flowView.SetNextText("결재");
        _flowView.SetNextInteractable(true);
    }

    private void HandleNextClicked()
    {
        if (_flowState.CurrentPhase.Value != GamePhase.RequestReview)
            return;

        if (_selectedStatus == null)
            return;

        var requestId = _flowState.SelectedRequestId.Value;

        if (string.IsNullOrEmpty(requestId))
            return;

        _requestSystem.ApplyDecision(requestId, _selectedStatus.Value);

        _selectedStatus = null;
    }

    private void ResetSelection()
    {
        _selectedStatus = null;

        _view.SetInteractionTagShow(false);
        _view.SetAgencyTagShow(false);

        _view.SetButtonsInteractable(true);

        _flowView.SetNextText("결재");
        _flowView.SetNextInteractable(false);
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