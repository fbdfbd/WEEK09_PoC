using System.Collections.Generic;
using System.Linq;

public readonly struct DeferredRequestRecord
{
    public string RequestId { get; }
    public int DeferredDay { get; }
    public int ReactivateDay { get; }

    public DeferredRequestRecord(string requestId, int deferredDay, int reactivateDay)
    {
        RequestId = requestId;
        DeferredDay = deferredDay;
        ReactivateDay = reactivateDay;
    }
}

public sealed class DeferredRequestStore
{
    private readonly List<DeferredRequestRecord> _records = new();
    private readonly HashSet<string> _activatedKeys = new();

    public void Add(string requestId, int deferredDay)
    {
        _records.Add(new DeferredRequestRecord(requestId, deferredDay, deferredDay + 1));
    }

    public IEnumerable<DeferredRequestRecord> GetRecordsForDay(int day)
    {
        return _records.Where(record => record.ReactivateDay == day && !HasActivated(record));
    }

    public void MarkActivated(DeferredRequestRecord record)
    {
        _activatedKeys.Add(GetKey(record));
    }

    private bool HasActivated(DeferredRequestRecord record)
    {
        return _activatedKeys.Contains(GetKey(record));
    }

    private string GetKey(DeferredRequestRecord record)
    {
        return $"{record.RequestId}:{record.ReactivateDay}";
    }
}
