using UnityEngine;
using VContainer.Unity;

public sealed class Bootstrap : IStartable
{
    private readonly SO_PoCDatabase _database;
    private readonly RuntimeDataBuilder _builder;
    private readonly RequestStore _requestStore;
    private readonly AgencyStore _agencyStore;
    private readonly RequestDaySystem _requestDaySystem;
    private readonly GameFlowState _flowState;

    public Bootstrap(
        SO_PoCDatabase database,
        RuntimeDataBuilder builder,
        RequestStore requestStore,
        AgencyStore agencyStore,
        RequestDaySystem requestDaySystem,
        GameFlowState flowState)
    {
        _database = database;
        _builder = builder;
        _requestStore = requestStore;
        _agencyStore = agencyStore;
        _requestDaySystem = requestDaySystem;
        _flowState = flowState;
    }

    public void Start()
    {
        var runtimeData = _builder.Build(_database);

        _requestStore.Initialize(runtimeData.Requests);
        _agencyStore.Initialize(runtimeData.Agencies);
        _requestDaySystem.StartDay(_flowState.CurrentDay.Value);

        Debug.Log($"Bootstrap complete / Requests: {runtimeData.Requests.Count}, Agencies: {runtimeData.Agencies.Count}");
        Debug.Log($"SelectedRequestId: {_flowState.SelectedRequestId.Value}");
        Debug.Log($"SelectedRequestIndex: {_flowState.SelectedRequestIndex.Value}");
        Debug.Log($"CurrentPhase: {_flowState.CurrentPhase.Value}");
    }
}
