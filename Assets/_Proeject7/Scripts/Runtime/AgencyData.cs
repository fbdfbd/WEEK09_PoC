using System.Collections.Generic;

public sealed class AgencyData
{
    public const int MinRelation = 0;
    public const int MaxRelation = 10;

    public string Id { get; }
    public string Name { get; }
    public string TooltipText { get; }

    public int Relation { get; private set; }

    private readonly HashSet<string> _staticTags;
    private readonly HashSet<string> _runtimeTags = new();

    public IReadOnlyCollection<string> StaticTags => _staticTags;
    public IReadOnlyCollection<string> RuntimeTags => _runtimeTags;

    public AgencyData(
        string id,
        string name,
        int initialRelation,
        string tooltipText,
        IEnumerable<string> staticTags)
    {
        Id = id;
        Name = name;
        TooltipText = tooltipText;
        Relation = ClampRelation(initialRelation);
        _staticTags = new HashSet<string>(staticTags);
    }

    public void SetRelation(int value)
    {
        Relation = ClampRelation(value);
    }

    public void ChangeRelation(int amount)
    {
        SetRelation(Relation + amount);
    }

    private static int ClampRelation(int value)
    {
        if (value < MinRelation)
            return MinRelation;

        if (value > MaxRelation)
            return MaxRelation;

        return value;
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
