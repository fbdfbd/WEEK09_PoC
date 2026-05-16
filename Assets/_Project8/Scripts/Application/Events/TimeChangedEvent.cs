namespace Project8.Application.Events
{
    public readonly struct TimeChangedEvent : IGameEvent
    {
        public readonly float RemainingSeconds;

        public TimeChangedEvent(float remainingSeconds)
        {
            RemainingSeconds = remainingSeconds;
        }
    }
}
