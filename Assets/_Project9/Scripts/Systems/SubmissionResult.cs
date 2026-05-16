using System.Collections.Generic;
using Project9.Data;

namespace Project9.Systems
{
    public sealed class SubmissionResult
    {
        public SubmissionResult(
            SubmitTargetDefinition target,
            int totalScore,
            int reputationDelta,
            IReadOnlyList<ParagraphScoreBreakdown> paragraphBreakdowns)
        {
            Target = target;
            TotalScore = totalScore;
            ReputationDelta = reputationDelta;
            ParagraphBreakdowns = paragraphBreakdowns;
        }

        public SubmitTargetDefinition Target { get; }
        public int TotalScore { get; }
        public int ReputationDelta { get; }
        public IReadOnlyList<ParagraphScoreBreakdown> ParagraphBreakdowns { get; }
    }
}
