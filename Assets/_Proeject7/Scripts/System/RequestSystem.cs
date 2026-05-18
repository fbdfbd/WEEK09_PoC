using System.Collections.Generic;

public sealed class RequestSystem
{
    private readonly RequestStore _requestStore;
    private readonly AgencyStore _agencyStore;
    private readonly RequestHistory _history;
    private readonly DeferredRequestStore _deferredRequestStore;
    private readonly RejectDecisionEvaluator _rejectDecisionEvaluator;
    private readonly RequestDecisionOutcomeEvaluator _decisionOutcomeEvaluator;
    private readonly PlayerTrustStore _playerTrustStore;
    private readonly GameFlowState _flowState;

    public RequestSystem(
        RequestStore requestStore,
        AgencyStore agencyStore,
        RequestHistory history,
        DeferredRequestStore deferredRequestStore,
        RejectDecisionEvaluator rejectDecisionEvaluator,
        RequestDecisionOutcomeEvaluator decisionOutcomeEvaluator,
        PlayerTrustStore playerTrustStore,
        GameFlowState flowState)
    {
        _requestStore = requestStore;
        _agencyStore = agencyStore;
        _history = history;
        _deferredRequestStore = deferredRequestStore;
        _rejectDecisionEvaluator = rejectDecisionEvaluator;
        _decisionOutcomeEvaluator = decisionOutcomeEvaluator;
        _playerTrustStore = playerTrustStore;
        _flowState = flowState;
    }

    public void ApplyDecision(string requestId, RequestDecisionDraft draft)
    {
        var request = _requestStore.Get(requestId);
        var status = draft.Status;

        request.SetStatus(status);
        _history.AddResult(requestId, status, _flowState.CurrentDay.Value);
        ApplyDecisionOutcome(request, draft);

        if (status != RequestStatus.Deferred)
            _requestStore.MarkResolved(requestId);

        switch (status)
        {
            case RequestStatus.Accepted:
                request.AddRuntimeTag("접수");
                request.AddRuntimeTag("기관배정필요");
                break;

            case RequestStatus.SupplementRequired:
                request.AddRuntimeTag("보완요구");
                break;

            case RequestStatus.Deferred:
                request.AddRuntimeTag("보류");
                _deferredRequestStore.Add(requestId, _flowState.CurrentDay.Value);
                break;

            case RequestStatus.Rejected:
                request.AddRuntimeTag("기각");
                ApplyRejectResult(request, draft);
                break;
        }
    }

    private void ApplyDecisionOutcome(RequestData request, RequestDecisionDraft draft)
    {
        var outcome = _decisionOutcomeEvaluator.Evaluate(request, draft);

        if (outcome.PlayerTrustDelta != 0)
            _playerTrustStore.ChangeTrust(outcome.PlayerTrustDelta);
    }

    private void ApplyRejectResult(RequestData request, RequestDecisionDraft draft)
    {
        if (!draft.HasRejectReason)
            return;

        var result = _rejectDecisionEvaluator.Evaluate(request, draft.RejectReason);
        request.AddRuntimeTag(result.IsValid ? "정당기각" : "부당기각");
        request.AddRuntimeTag($"기각사유:{draft.RejectReason}");

        if (result.AgencyRelationDelta == 0 || string.IsNullOrEmpty(request.RelatedAgencyId))
            return;

        var agency = _agencyStore.Get(request.RelatedAgencyId);
        agency.ChangeRelation(result.AgencyRelationDelta);
    }

    public void CompleteRequestReview()
    {
        var acceptedRequests = new List<RequestData>();

        foreach (var record in _history.GetRecords(_flowState.CurrentDay.Value, RequestStatus.Accepted))
        {
            var request = _requestStore.FindOrNull(record.RequestId);
            if (request != null)
                acceptedRequests.Add(request);
        }

        if (acceptedRequests.Count == 0)
        {
            _flowState.CurrentPhase.Value = GamePhase.Result;
            return;
        }

        _requestStore.SetAssignmentTargets(acceptedRequests);
        _flowState.SelectedRequestIndex.Value = 0;
        _flowState.SelectedRequestId.Value = acceptedRequests[0].Id;
        _flowState.CurrentPhase.Value = GamePhase.AgencyAssignment;
    }
}
