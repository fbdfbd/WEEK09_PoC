namespace Project8.Application.Events
{
    public readonly struct GameEndedEvent : IGameEvent
    {
        public readonly int FinalScore;

        public GameEndedEvent(int finalScore)
        {
            FinalScore = finalScore;
        }
    }
}
