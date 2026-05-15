using System.Collections.Generic;

public sealed class RequestData
{
    public string Id { get; }
    public string Title { get; }
    public string Body { get; }
    public string Summary { get; }

    public RequestStatus Status { get; private set; }

    private readonly HashSet<string> _staticTags;
    private readonly HashSet<string> _runtimeTags = new();

    public IReadOnlyCollection<string> StaticTags => _staticTags;
    public IReadOnlyCollection<string> RuntimeTags => _runtimeTags;

    public RequestData(
        string id,
        string title,
        string body,
        string summary,
        IEnumerable<string> staticTags)
    {
        Id = id;
        Title = title;
        Body = body;
        Summary = summary;

        Status = RequestStatus.Pending;
        _staticTags = new HashSet<string>(staticTags);
    }

    public void SetStatus(RequestStatus status)
    {
        Status = status;
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
}