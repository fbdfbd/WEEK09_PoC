using System.Collections.Generic;

namespace Project9.Presentation
{
    public sealed class SubmissionResultViewModel
    {
        public SubmissionResultViewModel(
            string targetId,
            string targetName,
            int totalScore,
            int reputationDelta,
            IReadOnlyList<ParagraphScoreViewModel> paragraphScores)
        {
            TargetId = targetId;
            TargetName = targetName;
            TotalScore = totalScore;
            ReputationDelta = reputationDelta;
            ParagraphScores = paragraphScores;
        }

        public string TargetId { get; }
        public string TargetName { get; }
        public int TotalScore { get; }
        public int ReputationDelta { get; }
        public IReadOnlyList<ParagraphScoreViewModel> ParagraphScores { get; }
    }
}
