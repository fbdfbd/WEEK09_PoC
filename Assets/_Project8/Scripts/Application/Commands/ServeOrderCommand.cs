namespace Project8.Application.Commands
{
    public readonly struct ServeOrderCommand : IGameCommand
    {
        public readonly string OrderInstanceId;

        public ServeOrderCommand(string orderInstanceId)
        {
            OrderInstanceId = orderInstanceId;
        }
    }
}
