using System.Collections.Generic;

public sealed class AgencyData
{
    public string Id { get; }
    public string Name { get; }

    public int Relation { get; private set; }

    private readonly HashSet<string> _staticTags;
    private readonly HashSet<string> _runtimeTags = new();

    public IReadOnlyCollection<string> StaticTags => _staticTags;
    public IReadOnlyCollection<string> RuntimeTags => _runtimeTags;

    public AgencyData(
        string id,
        string name,
        int initialRelation,
        IEnumerable<string> staticTags)
    {
        Id = id;
        Name = name;
        Relation = initialRelation;
        _staticTags = new HashSet<string>(staticTags);
    }

    public void SetRelation(int value)
    {
        Relation = value;
    }

    public void ChangeRelation(int amount)
    {
        Relation += amount;
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