using System.Collections.Generic;

public sealed class RequestDecisionDraftStore
{
    private readonly Dictionary<string, RequestStatus> _drafts = new();

    public void Set(string requestId, RequestStatus status)
    {
        _drafts[requestId] = status;
    }

    public bool TryGet(string requestId, out RequestStatus status)
    {
        return _drafts.TryGetValue(requestId, out status);
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
