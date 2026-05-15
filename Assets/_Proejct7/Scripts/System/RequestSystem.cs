public sealed class RequestSystem
{
    private readonly RequestStore _requestStore;
    private readonly GameFlowState _flowState;

    public RequestSystem(
        RequestStore requestStore,
        GameFlowState flowState)
    {
        _requestStore = requestStore;
        _flowState = flowState;
    }

    public void ApplyDecision(string requestId, RequestStatus status)
    {
        var request = _requestStore.Get(requestId);

        request.SetStatus(status);

        switch (status)
        {
            case RequestStatus.Accepted:
                request.AddRuntimeTag("접수완료");
                request.AddRuntimeTag("기관배정가능");
                _flowState.CurrentPhase.Value = GamePhase.AgencyAssignment;
                break;

            case RequestStatus.SupplementRequired:
                request.AddRuntimeTag("보완요구됨");
                _flowState.CurrentPhase.Value = GamePhase.Result;
                break;

            case RequestStatus.Deferred:
                request.AddRuntimeTag("보류됨");
                _flowState.CurrentPhase.Value = GamePhase.Result;
                break;

            case RequestStatus.Rejected:
                request.AddRuntimeTag("기각됨");
                _flowState.CurrentPhase.Value = GamePhase.Result;
                break;
        }
    }
}