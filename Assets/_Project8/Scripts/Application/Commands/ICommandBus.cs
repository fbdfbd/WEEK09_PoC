namespace Project8.Application.Commands
{
    public interface ICommandBus
    {
        void Register<TCommand>(ICommandHandler<TCommand> handler)
            where TCommand : IGameCommand;

        void Unregister<TCommand>(ICommandHandler<TCommand> handler)
            where TCommand : IGameCommand;

        void Publish<TCommand>(TCommand command)
            where TCommand : IGameCommand;
    }
}
