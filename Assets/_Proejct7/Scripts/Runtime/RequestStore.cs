using System.Collections.Generic;

/// <summary>
/// 해당 주차의 요청서(1) 저장 런타임 데이터 저장소
/// </summary>
public sealed class RequestStore 
{
    private readonly Dictionary<string, RequestData> _requests = new();

    public IReadOnlyCollection<RequestData> All => _requests.Values;

    public void Initialize(IEnumerable<RequestData> source)
    {
        _requests.Clear();

        foreach (var request in source)
            _requests[request.Id] = request;
    }

    public RequestData Get(string id)
    {
        return _requests[id];
    }
}
