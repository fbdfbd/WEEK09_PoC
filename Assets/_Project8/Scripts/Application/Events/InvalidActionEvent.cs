namespace Project8.Application.Events
{
    public readonly struct InvalidActionEvent : IGameEvent
    {
        public readonly string Message;

        public InvalidActionEvent(string message)
        {
            Message = message;
        }
    }
}
