using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatComponent
{
    private readonly Dictionary<string, float> _values = new();

    public IReadOnlyDictionary<string, float> Values => _values;

    public bool Has(string statId)
    {
        return IsValidStatId(statId) && _values.ContainsKey(statId);
    }

    public bool Has(SO_StatDefinition statDefinition)
    {
        return statDefinition != null && Has(statDefinition.Id);
    }

    public float Get(string statId)
    {
        return TryGet(statId, out var value) ? value : 0f;
    }

    public float Get(SO_StatDefinition statDefinition)
    {
        if (statDefinition == null)
            return 0f;

        return Get(statDefinition.Id);
    }

    public bool TryGet(string statId, out float value)
    {
        if (!IsValidStatId(statId))
        {
            value = 0f;
            return false;
        }

        return _values.TryGetValue(statId, out value);
    }

    public bool TryGet(SO_StatDefinition statDefinition, out float value)
    {
        if (statDefinition == null)
        {
            value = 0f;
            return false;
        }

        return TryGet(statDefinition.Id, out value);
    }

    public void Set(SO_StatDefinition statDefinition, float value)
    {
        ValidateDefinition(statDefinition);

        var clampedValue = Mathf.Clamp(value, statDefinition.Min, statDefinition.Max);
        _values[statDefinition.Id] = clampedValue;
    }

    public void Add(SO_StatDefinition statDefinition, float delta)
    {
        ValidateDefinition(statDefinition);

        var currentValue = Get(statDefinition.Id);
        Set(statDefinition, currentValue + delta);
    }

    public bool Remove(string statId)
    {
        if (!IsValidStatId(statId))
            return false;

        return _values.Remove(statId);
    }

    public bool Remove(SO_StatDefinition statDefinition)
    {
        if (statDefinition == null)
            return false;

        return Remove(statDefinition.Id);
    }

    public void Clear()
    {
        _values.Clear();
    }

    private static bool IsValidStatId(string statId)
    {
        return !string.IsNullOrWhiteSpace(statId);
    }

    private static void ValidateDefinition(SO_StatDefinition statDefinition)
    {
        if (statDefinition == null)
            throw new ArgumentNullException(nameof(statDefinition));

        if (string.IsNullOrWhiteSpace(statDefinition.Id))
            throw new ArgumentException("StatDefinition의 Id가 비어 있습니다.", nameof(statDefinition));
    }
}