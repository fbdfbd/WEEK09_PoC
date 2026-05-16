using System;
using R3;
using VContainer.Unity;

public sealed class AgencyRelationPresenter : IStartable, IDisposable
{
    private readonly AgencyRelationView _view;
    private readonly AgencyStore _agencyStore;
    private readonly GameFlowState _flowState;

    private readonly CompositeDisposable _disposables = new();

    public AgencyRelationPresenter(
        AgencyRelationView view,
        AgencyStore agencyStore,
        GameFlowState flowState)
    {
        _view = view;
        _agencyStore = agencyStore;
        _flowState = flowState;
    }

    public void Start()
    {
        _flowState.CurrentPhase
            .Subscribe(_ => Refresh())
            .AddTo(_disposables);

        _flowState.CurrentDay
            .Subscribe(_ => Refresh())
            .AddTo(_disposables);

        Refresh();
    }

    private void Refresh()
    {
        foreach (var agency in _agencyStore.All)
            _view.SetAgencyRelation(agency);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
