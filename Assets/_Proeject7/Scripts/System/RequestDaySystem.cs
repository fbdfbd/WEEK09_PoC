using System.Collections.Generic;
using System.Linq;

public sealed class RequestDaySystem
{
    private readonly SO_PoCDatabase _database;
    private readonly RequestStore _requestStore;
    private readonly RequestHistory _history;
    private readonly GameFlowState _flowState;

    public RequestDaySystem(
        SO_PoCDatabase database,
        RequestStore requestStore,
        RequestHistory history,
        GameFlowState flowState)
    {
        _database = database;
        _requestStore = requestStore;
        _history = history;
        _flowState = flowState;
    }

    public void StartDay(int day)
    {
        var activeRequests = BuildActiveRequests(day);
        _requestStore.SetActiveRequests(activeRequests);

        _flowState.CurrentDay.Value = day;
        _flowState.SelectedRequestIndex.Value = 0;
        _flowState.SelectedRequestId.Value = activeRequests.Count > 0
            ? activeRequests[0].Id
            : null;
        _flowState.CurrentPhase.Value = GamePhase.RequestReview;
    }

    private List<RequestData> BuildActiveRequests(int day)
    {
        var followUps = BuildFollowUpRequests();
        var baseRequests = _requestStore
            .GetBaseRequestsForDay(day)
            .Take(_database.RequestsPerDay);

        var activeRequests = followUps
            .Concat(baseRequests)
            .ToList();

        foreach (var request in activeRequests.Where(request => request.IsFollowUp))
            _history.MarkFollowUpActivated(request.ParentRequestId);

        return activeRequests;
    }

    private List<RequestData> BuildFollowUpRequests()
    {
        var followUps = new List<RequestData>();

        foreach (var record in _history.GetSupplementRequiredRecords())
        {
            if (_history.HasActivatedFollowUp(record.RequestId))
                continue;

            var parent = _requestStore.FindOrNull(record.RequestId);
            if (parent == null || string.IsNullOrEmpty(parent.SupplementFollowUpRequestId))
                continue;

            var followUp = _requestStore.FindOrNull(parent.SupplementFollowUpRequestId);
            if (followUp == null || _requestStore.IsResolved(followUp.Id))
                continue;

            followUps.Add(followUp);
        }

        return followUps
            .OrderByDescending(request => request.Priority)
            .ThenBy(request => request.Id)
            .ToList();
    }
}
