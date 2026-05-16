using System;
using System.Collections.Generic;

namespace Project8.Application.Commands
{
    public sealed class CommandBus : ICommandBus
    {
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();

        public void Register<TCommand>(ICommandHandler<TCommand> handler)
            where TCommand : IGameCommand
        {
            var commandType = typeof(TCommand);

            if (!_handlers.TryGetValue(commandType, out var handlers))
            {
                handlers = new List<object>();
                _handlers.Add(commandType, handlers);
            }

            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
        }

        public void Unregister<TCommand>(ICommandHandler<TCommand> handler)
            where TCommand : IGameCommand
        {
            var commandType = typeof(TCommand);

            if (!_handlers.TryGetValue(commandType, out var handlers))
            {
                return;
            }

            handlers.Remove(handler);
        }

        public void Publish<TCommand>(TCommand command)
            where TCommand : IGameCommand
        {
            var commandType = typeof(TCommand);

            if (!_handlers.TryGetValue(commandType, out var handlers))
            {
                return;
            }

            for (var i = 0; i < handlers.Count; i++)
            {
                var handler = (ICommandHandler<TCommand>)handlers[i];
                handler.Handle(command);
            }
        }
    }
}
