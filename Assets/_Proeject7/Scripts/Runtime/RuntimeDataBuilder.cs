public sealed class RuntimeDataBuilder
{
    public RuntimeData Build(SO_PoCDatabase database)
    {
        var runtimeData = new RuntimeData();

        foreach (var requestSo in database.Requests)
        {
            runtimeData.Requests.Add(BuildRequest(requestSo));
        }

        foreach (var agencySo in database.Agencies)
        {
            runtimeData.Agencies.Add(BuildAgency(agencySo));
        }

        return runtimeData;
    }

    private RequestData BuildRequest(SO_Request so)
    {
        return new RequestData(
            so.Id,
            so.Day,
            so.IsFollowUp,
            so.ParentRequestId,
            so.SupplementFollowUpRequestId,
            so.Priority,
            so.RelatedAgencyId,
            so.DeadlineDays,
            so.Title,
            so.Body,
            so.Summary,
            so.Tags,
            so.FactTags
        );
    }

    private AgencyData BuildAgency(SO_Agency so)
    {
        return new AgencyData(
            so.Id,
            so.Name,
            so.DefaultRelation,
            so.TooltipText,
            so.Tags
        );
    }
}
