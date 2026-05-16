using Project9.Data;

namespace Project9.Systems
{
    public sealed class ParagraphScoreBreakdown
    {
        public ParagraphScoreBreakdown(
            string paragraphId,
            ParagraphActionType actionType,
            int informationValueScore,
            int integrityScore,
            int exposureScore,
            int exposureReductionScore,
            int distortionPenalty,
            int fullMaskPenalty)
        {
            ParagraphId = paragraphId;
            ActionType = actionType;
            InformationValueScore = informationValueScore;
            IntegrityScore = integrityScore;
            ExposureScore = exposureScore;
            ExposureReductionScore = exposureReductionScore;
            DistortionPenalty = distortionPenalty;
            FullMaskPenalty = fullMaskPenalty;
        }

        public string ParagraphId { get; }
        public ParagraphActionType ActionType { get; }
        public int InformationValueScore { get; }
        public int IntegrityScore { get; }
        public int ExposureScore { get; }
        public int ExposureReductionScore { get; }
        public int DistortionPenalty { get; }
        public int FullMaskPenalty { get; }

        public int Total =>
            InformationValueScore +
            IntegrityScore +
            ExposureScore +
            ExposureReductionScore -
            DistortionPenalty -
            FullMaskPenalty;
    }
}
