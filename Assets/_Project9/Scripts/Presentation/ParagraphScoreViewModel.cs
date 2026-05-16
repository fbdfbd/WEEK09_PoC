using Project9.Data;

namespace Project9.Presentation
{
    public sealed class ParagraphScoreViewModel
    {
        public ParagraphScoreViewModel(
            string paragraphId,
            ParagraphActionType actionType,
            int total,
            int informationValueScore,
            int integrityScore,
            int exposureScore,
            int exposureReductionScore,
            int distortionPenalty,
            int fullMaskPenalty)
        {
            ParagraphId = paragraphId;
            ActionType = actionType;
            Total = total;
            InformationValueScore = informationValueScore;
            IntegrityScore = integrityScore;
            ExposureScore = exposureScore;
            ExposureReductionScore = exposureReductionScore;
            DistortionPenalty = distortionPenalty;
            FullMaskPenalty = fullMaskPenalty;
        }

        public string ParagraphId { get; }
        public ParagraphActionType ActionType { get; }
        public int Total { get; }
        public int InformationValueScore { get; }
        public int IntegrityScore { get; }
        public int ExposureScore { get; }
        public int ExposureReductionScore { get; }
        public int DistortionPenalty { get; }
        public int FullMaskPenalty { get; }
    }
}
