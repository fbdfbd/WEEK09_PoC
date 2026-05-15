using System.Collections.Generic;
using System.Linq;

public readonly struct RequestResultRecord
{
    public string RequestId { get; }
    public RequestStatus Status { get; }
    public int Day { get; }

    public RequestResultRecord(string requestId, RequestStatus status, int day)
    {
        RequestId = requestId;
        Status = status;
        Day = day;
    }
}

public sealed class RequestHistory
{
    private readonly List<RequestResultRecord> _records = new();
    private readonly HashSet<string> _activatedFollowUpParents = new();

    public IReadOnlyList<RequestResultRecord> Records => _records;

    public void AddResult(string requestId, RequestStatus status, int day)
    {
        _records.Add(new RequestResultRecord(requestId, status, day));
    }

    public IEnumerable<RequestResultRecord> GetSupplementRequiredRecords()
    {
        return _records.Where(record => record.Status == RequestStatus.SupplementRequired);
    }

    public IEnumerable<RequestResultRecord> GetRecords(int day, RequestStatus status)
    {
        return _records.Where(record => record.Day == day && record.Status == status);
    }

    public bool HasActivatedFollowUp(string parentRequestId)
    {
        return _activatedFollowUpParents.Contains(parentRequestId);
    }

    public void MarkFollowUpActivated(string parentRequestId)
    {
        _activatedFollowUpParents.Add(parentRequestId);
    }
}
