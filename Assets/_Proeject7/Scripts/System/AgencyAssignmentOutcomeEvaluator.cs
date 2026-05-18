public readonly struct AgencyAssignmentOutcome
{
    public int AgencyRelationDelta { get; }

    public AgencyAssignmentOutcome(int agencyRelationDelta)
    {
        AgencyRelationDelta = agencyRelationDelta;
    }
}

public sealed class AgencyAssignmentOutcomeEvaluator
{
    private const int ShouldRejectAssignmentRelationPenalty = -2;
    private const int MismatchedAgencyRelationPenalty = -2;

    private readonly RejectDecisionEvaluator _rejectDecisionEvaluator;

    public AgencyAssignmentOutcomeEvaluator(RejectDecisionEvaluator rejectDecisionEvaluator)
    {
        _rejectDecisionEvaluator = rejectDecisionEvaluator;
    }

    public AgencyAssignmentOutcome Evaluate(RequestData request, string assignedAgencyId)
    {
        var relationDelta = 0;

        if (_rejectDecisionEvaluator.ShouldReject(request))
            relationDelta += ShouldRejectAssignmentRelationPenalty;

        if (IsMismatchedAgency(request, assignedAgencyId))
            relationDelta += MismatchedAgencyRelationPenalty;

        return new AgencyAssignmentOutcome(relationDelta);
    }

    private static bool IsMismatchedAgency(RequestData request, string assignedAgencyId)
    {
        return !string.IsNullOrEmpty(request.RelatedAgencyId)
            && request.RelatedAgencyId != assignedAgencyId;
    }
}
