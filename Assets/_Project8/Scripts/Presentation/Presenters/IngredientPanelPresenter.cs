using System;
using System.Collections.Generic;
using Project8.Application.Commands;
using Project8.Application.Events;
using Project8.Application.Services;
using Project8.Domain.Model;
using Project8.Presentation.Views;
using VContainer.Unity;

namespace Project8.Presentation.Presenters
{
    public sealed class IngredientPanelPresenter :
        IStartable,
        IDisposable,
        IEventHandler<IngredientAppliedEvent>,
        IEventHandler<InvalidActionEvent>
    {
        private readonly IIngredientPanelView _view;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IRuntimeDataFactory _factory;
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;

        public IngredientPanelPresenter(
            IIngredientPanelView view,
            IIngredientRepository ingredientRepository,
            IRuntimeDataFactory factory,
            ICommandBus commandBus,
            IEventBus eventBus)
        {
            _view = view;
            _ingredientRepository = ingredientRepository;
            _factory = factory;
            _commandBus = commandBus;
            _eventBus = eventBus;
        }

        public void Start()
        {
            _view.IngredientClicked += OnIngredientClicked;
            _eventBus.Register<IngredientAppliedEvent>(this);
            _eventBus.Register<InvalidActionEvent>(this);

            _view.SetIngredients(CreateIngredientModels());
        }

        public void Handle(IngredientAppliedEvent gameEvent)
        {
            _view.PlayIngredientUsed(gameEvent.DisplayName);
        }

        public void Handle(InvalidActionEvent gameEvent)
        {
            _view.PlayInvalidAction(gameEvent.Message);
        }

        public void Dispose()
        {
            _view.IngredientClicked -= OnIngredientClicked;
            _eventBus.Unregister<IngredientAppliedEvent>(this);
            _eventBus.Unregister<InvalidActionEvent>(this);
        }

        private void OnIngredientClicked(string ingredientId)
        {
            _commandBus.Publish(new AddIngredientCommand(ingredientId));
        }

        private IReadOnlyList<IngredientRuntimeModel> CreateIngredientModels()
        {
            var definitions = _ingredientRepository.GetAll();
            var ingredients = new List<IngredientRuntimeModel>();

            for (var i = 0; i < definitions.Count; i++)
            {
                if (definitions[i] != null)
                {
                    ingredients.Add(_factory.CreateIngredient(definitions[i]));
                }
            }

            return ingredients;
        }
    }
}
