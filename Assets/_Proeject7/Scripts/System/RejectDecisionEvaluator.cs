public readonly struct RejectDecisionResult
{
    public bool IsValid { get; }
    public int AgencyRelationDelta { get; }

    public RejectDecisionResult(bool isValid, int agencyRelationDelta)
    {
        IsValid = isValid;
        AgencyRelationDelta = agencyRelationDelta;
    }
}

public sealed class RejectDecisionEvaluator
{
    private const int InvalidRejectPenalty = -1;

    public RejectDecisionResult Evaluate(RequestData request, RejectReason reason)
    {
        var isValid = CanReject(request, reason);
        return new RejectDecisionResult(
            isValid,
            isValid ? 0 : InvalidRejectPenalty
        );
    }

    private bool CanReject(RequestData request, RejectReason reason)
    {
        return reason switch
        {
            RejectReason.PersonalInformation =>
                request.HasFact(RequestFactTag.ContainsPersonalInfo)
                && !request.HasFact(RequestFactTag.PublicInterestHigh),

            RejectReason.NationalSecurity =>
                request.HasFact(RequestFactTag.SecuritySensitive),

            _ => false
        };
    }

    public bool ShouldReject(RequestData request)
    {
        return CanReject(request, RejectReason.PersonalInformation)
            || CanReject(request, RejectReason.NationalSecurity);
    }
}
