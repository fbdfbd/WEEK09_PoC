using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 해당 주차의 요청서(1) 저장 런타임 데이터 저장소
/// </summary>
public sealed class RequestStore 
{
    private readonly Dictionary<string, RequestData> _requests = new();
    private readonly List<RequestData> _activeRequests = new();
    private readonly List<RequestData> _assignmentTargets = new();
    private readonly HashSet<string> _resolvedRequestIds = new();
    private readonly HashSet<string> _assignedRequestIds = new();

    public IReadOnlyCollection<RequestData> All => _requests.Values;
    public IReadOnlyList<RequestData> ActiveRequests => _activeRequests;
    public IReadOnlyList<RequestData> AssignmentTargets => _assignmentTargets;
    public int ActiveCount => _activeRequests.Count;
    public int AssignmentTargetCount => _assignmentTargets.Count;

    public void Initialize(IEnumerable<RequestData> source)
    {
        _requests.Clear();
        _activeRequests.Clear();
        _assignmentTargets.Clear();
        _resolvedRequestIds.Clear();
        _assignedRequestIds.Clear();

        foreach (var request in source)
            _requests[request.Id] = request;
    }

    public RequestData Get(string id)
    {
        return _requests[id];
    }

    public RequestData GetActive(int index)
    {
        return _activeRequests[index];
    }

    public RequestData GetAssignmentTarget(int index)
    {
        return _assignmentTargets[index];
    }

    public void SetActiveRequests(IEnumerable<RequestData> requests)
    {
        _activeRequests.Clear();
        _activeRequests.AddRange(requests);
    }

    public void MarkResolved(string requestId)
    {
        _resolvedRequestIds.Add(requestId);
    }

    public void SetAssignmentTargets(IEnumerable<RequestData> requests)
    {
        _assignmentTargets.Clear();
        _assignmentTargets.AddRange(requests);
    }

    public void MarkAssignmentCompleted(string requestId)
    {
        _assignedRequestIds.Add(requestId);
    }

    public bool IsAssignmentCompleted(string requestId)
    {
        return _assignedRequestIds.Contains(requestId);
    }

    public bool IsResolved(string requestId)
    {
        return _resolvedRequestIds.Contains(requestId);
    }

    public bool TryGetNextPendingIndex(int currentIndex, out int nextIndex)
    {
        nextIndex = -1;

        for (var i = currentIndex + 1; i < _activeRequests.Count; i++)
        {
            if (!IsResolved(_activeRequests[i].Id))
            {
                nextIndex = i;
                return true;
            }
        }

        for (var i = 0; i < _activeRequests.Count; i++)
        {
            if (!IsResolved(_activeRequests[i].Id))
            {
                nextIndex = i;
                return true;
            }
        }

        return false;
    }

    public bool TryGetNextAssignmentIndex(int currentIndex, out int nextIndex)
    {
        nextIndex = -1;

        for (var i = currentIndex + 1; i < _assignmentTargets.Count; i++)
        {
            if (!IsAssignmentCompleted(_assignmentTargets[i].Id))
            {
                nextIndex = i;
                return true;
            }
        }

        for (var i = 0; i < _assignmentTargets.Count; i++)
        {
            if (!IsAssignmentCompleted(_assignmentTargets[i].Id))
            {
                nextIndex = i;
                return true;
            }
        }

        return false;
    }

    public RequestData FindOrNull(string id)
    {
        return _requests.TryGetValue(id, out var request) ? request : null;
    }

    public IEnumerable<RequestData> GetBaseRequestsForDay(int day)
    {
        return _requests.Values
            .Where(request => request.Day == day && !request.IsFollowUp && !IsResolved(request.Id))
            .OrderByDescending(request => request.Priority)
            .ThenBy(request => request.Id);
    }
}
