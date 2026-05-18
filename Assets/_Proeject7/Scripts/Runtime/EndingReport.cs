public sealed class EndingReport
{
    public string Title { get; }
    public string PlayerOutcomeText { get; }
    public string AchievementText { get; }
    public string TrustText { get; }
    public string AgencyRelationsText { get; }
    public string IncidentsText { get; }

    public EndingReport(
        string title,
        string playerOutcomeText,
        string achievementText,
        string trustText,
        string agencyRelationsText,
        string incidentsText)
    {
        Title = title;
        PlayerOutcomeText = playerOutcomeText;
        AchievementText = achievementText;
        TrustText = trustText;
        AgencyRelationsText = agencyRelationsText;
        IncidentsText = incidentsText;
    }
}
