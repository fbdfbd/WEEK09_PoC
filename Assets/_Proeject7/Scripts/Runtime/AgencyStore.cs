using System.Collections.Generic;

/// <summary>
/// 기관 런타임 데이터 저장소
/// 기관의 고정데이터 및 가변데이터 저장소
/// </summary>
public sealed class AgencyStore
{
    private readonly Dictionary<string, AgencyData> agencies = new();

    public IReadOnlyCollection<AgencyData> All => agencies.Values;

    public void Initialize(IEnumerable<AgencyData> source)
    {
        agencies.Clear();

        foreach (var agency in source)
            agencies[agency.Id] = agency;
    }

    public AgencyData Get(string id)
    {
        return agencies[id];
    }
}
