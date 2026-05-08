namespace App.Gameplay.Reports
{
    public sealed class ReportEntry
    {
        public ReportEntry(int weekIndex, string title, string body)
        {
            WeekIndex = weekIndex;
            Title = title;
            Body = body;
        }

        public int WeekIndex { get; }
        public string Title { get; }
        public string Body { get; }
    }
}
