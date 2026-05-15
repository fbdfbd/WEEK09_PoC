using System.Collections.Generic;

public sealed class RequestSystem
{
    private readonly RequestStore _requestStore;
    private readonly RequestHistory _history;
    private readonly GameFlowState _flowState;

    public RequestSystem(
        RequestStore requestStore,
        RequestHistory history,
        GameFlowState flowState)
    {
        _requestStore = requestStore;
        _history = history;
        _flowState = flowState;
    }

    public void ApplyDecision(string requestId, RequestStatus status)
    {
        var request = _requestStore.Get(requestId);

        request.SetStatus(status);
        _history.AddResult(requestId, status, _flowState.CurrentDay.Value);
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
                break;

            case RequestStatus.Rejected:
                request.AddRuntimeTag("기각");
                break;
        }
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
