using System.Collections.Generic;

public sealed class RequestData
{
    public string Id { get; }
    public int Day { get; }
    public bool IsFollowUp { get; }
    public string ParentRequestId { get; }
    public string SupplementFollowUpRequestId { get; }
    public string DeferredFollowUpRequestId { get; }
    public int Priority { get; }
    public string RelatedAgencyId { get; }
    public int DeadlineDays { get; }
    public int RemainingDays { get; private set; }
    public string Title { get; }
    public string Body { get; }
    public string Summary { get; }

    public RequestStatus Status { get; private set; }

    private readonly HashSet<string> _staticTags;
    private readonly HashSet<string> _runtimeTags = new();
    private readonly HashSet<RequestFactTag> _factTags;

    public IReadOnlyCollection<string> StaticTags => _staticTags;
    public IReadOnlyCollection<string> RuntimeTags => _runtimeTags;
    public IReadOnlyCollection<RequestFactTag> FactTags => _factTags;
    public bool CanDefer => RemainingDays > 1;

    public RequestData(
        string id,
        int day,
        bool isFollowUp,
        string parentRequestId,
        string supplementFollowUpRequestId,
        string deferredFollowUpRequestId,
        int priority,
        string relatedAgencyId,
        int deadlineDays,
        string title,
        string body,
        string summary,
        IEnumerable<string> staticTags,
        IEnumerable<RequestFactTag> factTags)
    {
        Id = id;
        Day = day;
        IsFollowUp = isFollowUp;
        ParentRequestId = parentRequestId;
        SupplementFollowUpRequestId = supplementFollowUpRequestId;
        DeferredFollowUpRequestId = deferredFollowUpRequestId;
        Priority = priority;
        RelatedAgencyId = relatedAgencyId;
        DeadlineDays = deadlineDays > 0 ? deadlineDays : 2;
        RemainingDays = DeadlineDays;
        Title = title;
        Body = body;
        Summary = summary;

        Status = RequestStatus.Pending;
        _staticTags = new HashSet<string>(staticTags);
        _factTags = new HashSet<RequestFactTag>(factTags);
    }

    public void SetStatus(RequestStatus status)
    {
        Status = status;
    }

    public void ResetForReview()
    {
        Status = RequestStatus.Pending;
    }

    public void DecreaseRemainingDays()
    {
        if (RemainingDays > 0)
            RemainingDays--;
    }

    public void AddRuntimeTag(string tag)
    {
        _runtimeTags.Add(tag);
    }

    public void RemoveRuntimeTag(string tag)
    {
        _runtimeTags.Remove(tag);
    }

    public bool HasTag(string tag)
    {
        return _staticTags.Contains(tag) || _runtimeTags.Contains(tag);
    }

    public bool HasFact(RequestFactTag factTag)
    {
        return _factTags.Contains(factTag);
    }
}
