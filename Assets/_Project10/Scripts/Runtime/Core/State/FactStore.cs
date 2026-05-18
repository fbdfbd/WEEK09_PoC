using System;
using System.Collections.Generic;

[Serializable]
public class FactStore
{
    private readonly HashSet<string> _facts = new();

    public IReadOnlyCollection<string> Facts => _facts;

    public bool Has(string factId)
    {
        return IsValidFactId(factId) && _facts.Contains(factId);
    }

    public void Add(string factId)
    {
        ValidateFactId(factId);
        _facts.Add(factId);
    }

    public bool Remove(string factId)
    {
        if (!IsValidFactId(factId))
            return false;

        return _facts.Remove(factId);
    }

    public void Clear()
    {
        _facts.Clear();
    }

    private static bool IsValidFactId(string factId)
    {
        return !string.IsNullOrWhiteSpace(factId);
    }

    private static void ValidateFactId(string factId)
    {
        if (string.IsNullOrWhiteSpace(factId))
            throw new ArgumentException("FactId는 비어 있을 수 없습니다.", nameof(factId));
    }
}