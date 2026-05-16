using Project8.Application.Commands;
using Project8.Application.Events;
using Project8.Application.Services;
using Project8.Application.Systems;
using Project8.Domain.Data;
using Project8.Domain.Model;
using Project8.Domain.Rules;
using Project8.Infrastructure.Repositories;
using Project8.Infrastructure.UnityAdapters;
using Project8.Presentation.Presenters;
using Project8.Presentation.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Project8.Infrastructure.Installers
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Config")]
        [SerializeField] private SO_GameConfig _gameConfig;

        [Header("Data")]
        [SerializeField] private SO_IngredientDefinition[] _ingredients;
        [SerializeField] private SO_OrderDefinition[] _orders;

        [Header("Views")]
        [SerializeField] private PotView _potView;
        [SerializeField] private IngredientPanelView _ingredientPanelView;
        [SerializeField] private OrderQueueView _orderQueueView;
        [SerializeField] private HudView _hudView;

        protected override void Awake()
        {
            EnsureNewInputSystemEventModule();
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterData(builder);
            RegisterCore(builder);
            RegisterSystems(builder);
            RegisterViews(builder);
            RegisterPresenters(builder);
            RegisterEntryPoints(builder);
        }

        private void RegisterData(IContainerBuilder builder)
        {
            builder.RegisterInstance(_gameConfig);
            builder.RegisterInstance(_ingredients);
            builder.RegisterInstance(_orders);
        }

        private void RegisterCore(IContainerBuilder builder)
        {
            builder.Register<GameRuntimeModel>(Lifetime.Singleton);
            builder.Register<TasteScoreCalculator>(Lifetime.Singleton);

            builder.Register<ICommandBus, CommandBus>(Lifetime.Singleton);
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
            builder.Register<IRuntimeDataFactory, RuntimeDataFactory>(Lifetime.Singleton);
            builder.Register<IRandomService, UnityRandomService>(Lifetime.Singleton);

            builder.Register<IIngredientRepository, IngredientRepository>(Lifetime.Singleton);
            builder.Register<IOrderRepository, OrderRepository>(Lifetime.Singleton);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.Register<GameFlowSystem>(Lifetime.Singleton);
            builder.Register<PotSimulateSystem>(Lifetime.Singleton);
            builder.Register<IngredientApplySystem>(Lifetime.Singleton);
            builder.Register<OrderSpawnSystem>(Lifetime.Singleton);
            builder.Register<OrderPatienceSystem>(Lifetime.Singleton);
            builder.Register<ServeSystem>(Lifetime.Singleton);
            builder.Register<ScoreSystem>(Lifetime.Singleton);
        }

        private void RegisterViews(IContainerBuilder builder)
        {
            builder.RegisterInstance<IPotView>(_potView);
            builder.RegisterInstance<IIngredientPanelView>(_ingredientPanelView);
            builder.RegisterInstance<IOrderQueueView>(_orderQueueView);
            builder.RegisterInstance<IHudView>(_hudView);
        }

        private void RegisterPresenters(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<PotPresenter>();
            builder.RegisterEntryPoint<IngredientPanelPresenter>();
            builder.RegisterEntryPoint<OrderQueuePresenter>();
            builder.RegisterEntryPoint<HudPresenter>();
        }

        private void RegisterEntryPoints(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameLoopEntryPoint>();
        }

        private static void EnsureNewInputSystemEventModule()
        {
            var eventSystem = FindFirstObjectByType<EventSystem>();

            if (eventSystem == null)
            {
                return;
            }

            var oldModule = eventSystem.GetComponent<StandaloneInputModule>();

            if (oldModule != null)
            {
                Destroy(oldModule);
            }

            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }
        }
    }
}
