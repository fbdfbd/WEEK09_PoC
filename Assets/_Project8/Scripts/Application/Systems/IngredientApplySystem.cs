using System;
using Project8.Application.Commands;
using Project8.Application.Events;
using Project8.Application.Services;
using Project8.Domain.Data;
using Project8.Domain.Model;
using Project8.Domain.Rules;

namespace Project8.Application.Systems
{
    public sealed class IngredientApplySystem :
        ICommandHandler<AddIngredientCommand>,
        IDisposable
    {
        private readonly GameRuntimeModel _game;
        private readonly SO_GameConfig _config;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IRuntimeDataFactory _factory;
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;

        public IngredientApplySystem(
            GameRuntimeModel game,
            SO_GameConfig config,
            IIngredientRepository ingredientRepository,
            IRuntimeDataFactory factory,
            ICommandBus commandBus,
            IEventBus eventBus)
        {
            _game = game;
            _config = config;
            _ingredientRepository = ingredientRepository;
            _factory = factory;
            _commandBus = commandBus;
            _eventBus = eventBus;

            _commandBus.Register(this);
        }

        public void Handle(AddIngredientCommand command)
        {
            if (!_game.IsPlaying)
            {
                return;
            }

            if (!_ingredientRepository.TryGetById(command.IngredientId, out var definition))
            {
                _eventBus.Publish(new InvalidActionEvent("재료를 찾을 수 없습니다."));
                return;
            }

            var ingredient = _factory.CreateIngredient(definition);
            var result = PotRules.ApplyIngredient(
                _game.Pot,
                ingredient,
                _config.FriedRiceThreshold);

            if (!result.IsSuccess)
            {
                _eventBus.Publish(new InvalidActionEvent(result.Message));
                return;
            }

            _eventBus.Publish(new PotStateChangedEvent(_game.Pot));
            _eventBus.Publish(new IngredientAppliedEvent(
                ingredient.Id,
                ingredient.DisplayName));
        }

        public void Dispose()
        {
            _commandBus.Unregister<AddIngredientCommand>(this);
        }
    }
}
