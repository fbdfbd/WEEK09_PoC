using System.Collections.Generic;

public sealed class RuntimeData
{
    public List<RequestData> Requests { get; } = new();
    public List<AgencyData> Agencies { get; } = new();
}