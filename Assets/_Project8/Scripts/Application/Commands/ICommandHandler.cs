namespace Project8.Application.Commands
{
    public interface ICommandHandler<in TCommand>
        where TCommand : IGameCommand
    {
        void Handle(TCommand command);
    }
}
