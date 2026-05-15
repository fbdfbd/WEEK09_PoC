using System.Collections.Generic;

public sealed class AgencyAssignmentDraftStore
{
    private readonly Dictionary<string, string> _drafts = new();

    public void Set(string requestId, string agencyId)
    {
        _drafts[requestId] = agencyId;
    }

    public bool TryGet(string requestId, out string agencyId)
    {
        return _drafts.TryGetValue(requestId, out agencyId);
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
