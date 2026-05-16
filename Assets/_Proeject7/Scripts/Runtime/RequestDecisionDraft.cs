public readonly struct RequestDecisionDraft
{
    public RequestStatus Status { get; }
    public RejectReason RejectReason { get; }
    public bool HasRejectReason { get; }

    public RequestDecisionDraft(RequestStatus status)
    {
        Status = status;
        RejectReason = default;
        HasRejectReason = false;
    }

    public RequestDecisionDraft(RequestStatus status, RejectReason rejectReason)
    {
        Status = status;
        RejectReason = rejectReason;
        HasRejectReason = true;
    }
}
