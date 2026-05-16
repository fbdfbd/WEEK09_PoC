using System.Collections.Generic;

namespace Project9.Presentation
{
    public sealed class ReportViewModel
    {
        public ReportViewModel(
            string title,
            string summary,
            IReadOnlyList<ParagraphViewModel> paragraphs,
            IReadOnlyList<TargetViewModel> targets,
            string selectedTargetId)
        {
            Title = title;
            Summary = summary;
            Paragraphs = paragraphs;
            Targets = targets;
            SelectedTargetId = selectedTargetId;
        }

        public string Title { get; }
        public string Summary { get; }
        public IReadOnlyList<ParagraphViewModel> Paragraphs { get; }
        public IReadOnlyList<TargetViewModel> Targets { get; }
        public string SelectedTargetId { get; }
    }
}
