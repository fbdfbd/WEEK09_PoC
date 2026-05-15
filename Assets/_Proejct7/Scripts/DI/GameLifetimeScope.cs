using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class GameLifetimeScope : LifetimeScope
{
    [Header("Database")]
    [SerializeField] private SO_PoCDatabase _database;

    [Header("Views")]
    [SerializeField] private RequestView _requestView;
    [SerializeField] private FlowControlView _flowControlView;
    [SerializeField] private HudView _hudView;

    protected override void Configure(IContainerBuilder builder)
    {
        // SO 원본 데이터
        builder.RegisterInstance(_database);

        // 씬에 배치된 View
        builder.RegisterInstance(_requestView);
        builder.RegisterInstance(_flowControlView);
        builder.RegisterInstance(_hudView);

        // State
        builder.Register<GameFlowState>(Lifetime.Singleton);

        // Stores
        builder.Register<RequestStore>(Lifetime.Singleton);
        builder.Register<AgencyStore>(Lifetime.Singleton);

        // Builders
        builder.Register<RuntimeDataBuilder>(Lifetime.Singleton);

        // Systems
        builder.Register<RequestSystem>(Lifetime.Singleton);

        // Providers
        builder.Register<PhaseTextProvider>(Lifetime.Singleton);

        // EntryPoints
        builder.RegisterEntryPoint<Bootstrap>();
        builder.RegisterEntryPoint<RequestPresenter>();
        builder.RegisterEntryPoint<HudPresenter>();
    }
}