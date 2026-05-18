public sealed class AgencyAssignmentSystem
{
    private readonly RequestStore _requestStore;
    private readonly AgencyStore _agencyStore;
    private readonly AgencyAssignmentOutcomeEvaluator _assignmentOutcomeEvaluator;
    private readonly GameFlowState _flowState;

    public AgencyAssignmentSystem(
        RequestStore requestStore,
        AgencyStore agencyStore,
        AgencyAssignmentOutcomeEvaluator assignmentOutcomeEvaluator,
        GameFlowState flowState)
    {
        _requestStore = requestStore;
        _agencyStore = agencyStore;
        _assignmentOutcomeEvaluator = assignmentOutcomeEvaluator;
        _flowState = flowState;
    }

    public void AssignAgency(string requestId, string agencyId)
    {
        var request = _requestStore.Get(requestId);
        var agency = _agencyStore.Get(agencyId);

        request.AddRuntimeTag("기관배정완료");
        request.AddRuntimeTag($"기관:{agency.Id}");

        var outcome = _assignmentOutcomeEvaluator.Evaluate(request, agencyId);
        if (outcome.AgencyRelationDelta != 0)
            agency.ChangeRelation(outcome.AgencyRelationDelta);

        _requestStore.MarkAssignmentCompleted(requestId);
    }

    public void CompleteAgencyAssignment()
    {
        _flowState.CurrentPhase.Value = GamePhase.Result;
    }
}
