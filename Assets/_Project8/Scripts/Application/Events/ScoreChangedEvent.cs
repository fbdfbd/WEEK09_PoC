namespace Project8.Application.Events
{
    public readonly struct ScoreChangedEvent : IGameEvent
    {
        public readonly int Score;

        public ScoreChangedEvent(int score)
        {
            Score = score;
        }
    }
}
