public class BattleLogEntry
{
    public string Message { get; private set; }

    public BattleLogEntry(string message)
    {
        Message = message;
    }
}
