using System.Collections.Generic;

public sealed class RequestDecisionDraftStore
{
    private readonly Dictionary<string, RequestDecisionDraft> _drafts = new();

    public void Set(string requestId, RequestStatus status)
    {
        _drafts[requestId] = new RequestDecisionDraft(status);
    }

    public void SetRejected(string requestId, RejectReason rejectReason)
    {
        _drafts[requestId] = new RequestDecisionDraft(RequestStatus.Rejected, rejectReason);
    }

    public bool TryGet(string requestId, out RequestDecisionDraft draft)
    {
        return _drafts.TryGetValue(requestId, out draft);
    }

    public bool Has(string requestId)
    {
        return _drafts.ContainsKey(requestId);
    }

    public void Clear(string requestId)
    {
        _drafts.Remove(requestId);
    }
}
