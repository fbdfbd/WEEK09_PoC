public readonly struct RequestDecisionOutcome
{
    public int PlayerTrustDelta { get; }

    public RequestDecisionOutcome(int playerTrustDelta)
    {
        PlayerTrustDelta = playerTrustDelta;
    }
}

public sealed class RequestDecisionOutcomeEvaluator
{
    private const int CorrectRejectTrustReward = 5;
    private const int InvalidRejectTrustPenalty = -8;
    private const int ShouldRejectAcceptedTrustPenalty = -10;

    private readonly RejectDecisionEvaluator _rejectDecisionEvaluator;

    public RequestDecisionOutcomeEvaluator(RejectDecisionEvaluator rejectDecisionEvaluator)
    {
        _rejectDecisionEvaluator = rejectDecisionEvaluator;
    }

    public RequestDecisionOutcome Evaluate(RequestData request, RequestDecisionDraft draft)
    {
        if (draft.Status == RequestStatus.Accepted && _rejectDecisionEvaluator.ShouldReject(request))
            return new RequestDecisionOutcome(ShouldRejectAcceptedTrustPenalty);

        if (draft.Status != RequestStatus.Rejected || !draft.HasRejectReason)
            return new RequestDecisionOutcome(0);

        var rejectResult = _rejectDecisionEvaluator.Evaluate(request, draft.RejectReason);
        return new RequestDecisionOutcome(
            rejectResult.IsValid ? CorrectRejectTrustReward : InvalidRejectTrustPenalty
        );
    }
}
