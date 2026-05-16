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
    [SerializeField] private IndexSelectorView _requestIndexSelectorView;
    [SerializeField] private AgencyRelationView _agencyRelationView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_database);

        builder.RegisterInstance(_requestView);
        builder.RegisterInstance(_flowControlView);
        builder.RegisterInstance(_hudView);
        builder.RegisterInstance(_requestIndexSelectorView);

        if (_agencyRelationView != null)
            builder.RegisterInstance(_agencyRelationView);

        builder.Register<GameFlowState>(Lifetime.Singleton);

        builder.Register<RequestStore>(Lifetime.Singleton);
        builder.Register<AgencyStore>(Lifetime.Singleton);
        builder.Register<RequestHistory>(Lifetime.Singleton);
        builder.Register<DeferredRequestStore>(Lifetime.Singleton);
        builder.Register<RequestDecisionDraftStore>(Lifetime.Singleton);
        builder.Register<AgencyAssignmentDraftStore>(Lifetime.Singleton);

        builder.Register<RuntimeDataBuilder>(Lifetime.Singleton);

        builder.Register<RequestSystem>(Lifetime.Singleton);
        builder.Register<RequestDaySystem>(Lifetime.Singleton);
        builder.Register<AgencyAssignmentSystem>(Lifetime.Singleton);
        builder.Register<RejectDecisionEvaluator>(Lifetime.Singleton);

        builder.Register<PhaseTextProvider>(Lifetime.Singleton);
        builder.Register<RequestTextProvider>(Lifetime.Singleton);

        builder.RegisterEntryPoint<Bootstrap>();
        builder.RegisterEntryPoint<RequestListPresenter>();
        builder.RegisterEntryPoint<RequestPresenter>();
        builder.RegisterEntryPoint<AgencyAssignmentPresenter>();
        builder.RegisterEntryPoint<DayFlowPresenter>();
        builder.RegisterEntryPoint<HudPresenter>();

        if (_agencyRelationView != null)
            builder.RegisterEntryPoint<AgencyRelationPresenter>();
    }
}
