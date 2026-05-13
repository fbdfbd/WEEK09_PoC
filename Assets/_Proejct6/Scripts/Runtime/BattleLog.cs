using System.Collections.Generic;

public class BattleLog
{
    private readonly List<BattleLogEntry> entries = new List<BattleLogEntry>();

    public IReadOnlyList<BattleLogEntry> Entries => entries;

    public void Add(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        entries.Add(new BattleLogEntry(message));
    }

    public void Clear()
    {
        entries.Clear();
    }
}
