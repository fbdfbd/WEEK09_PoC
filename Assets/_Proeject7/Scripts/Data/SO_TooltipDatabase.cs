using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class TooltipEntry
{
    [SerializeField] private TooltipKey _key;
    [SerializeField][TextArea] private string _text;

    public TooltipKey Key => _key;
    public string Text => _text;
}

[CreateAssetMenu(fileName = "SO_TooltipDatabase", menuName = "PoC7/SO_TooltipDatabase")]
public sealed class SO_TooltipDatabase : ScriptableObject
{
    [SerializeField] private List<TooltipEntry> _entries = new();

    public bool TryGetText(TooltipKey key, out string text)
    {
        foreach (var entry in _entries)
        {
            if (entry == null || entry.Key != key)
                continue;

            text = entry.Text;
            return !string.IsNullOrWhiteSpace(text);
        }

        text = string.Empty;
        return false;
    }
}
