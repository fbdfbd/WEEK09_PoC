using System;
using System.Collections.Generic;
using Project9.Data;
using Project9.Runtime;
using UnityEngine;

namespace Project9.Systems
{
    public sealed class SubmissionScoringSystem
    {
        private const int ReputationScoreDivisor = 35;
        private const int MinReputationDelta = -25;
        private const int MaxReputationDelta = 25;

        public SubmissionResult CalculateSelectedTarget(ReportSessionState session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            return Calculate(session, session.SelectedSubmitTarget);
        }

        public SubmissionResult Calculate(ReportSessionState session, SubmitTargetDefinition target)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var breakdowns = new List<ParagraphScoreBreakdown>();
            var totalScore = 0;

            foreach (var paragraph in session.Paragraphs)
            {
                var breakdown = CalculateParagraph(paragraph, target.ScoringProfile);
                breakdowns.Add(breakdown);
                totalScore += breakdown.Total;
            }

            var reputationDelta = Mathf.Clamp(
                Mathf.RoundToInt((float)totalScore / ReputationScoreDivisor),
                MinReputationDelta,
                MaxReputationDelta);

            return new SubmissionResult(target, totalScore, reputationDelta, breakdowns);
        }

        private static ParagraphScoreBreakdown CalculateParagraph(
            ParagraphRuntimeState paragraph,
            SubmitTargetScoringProfile profile)
        {
            var definition = paragraph.Definition;
            var value = definition.InformationValue;
            var baseSensitivity = definition.Sensitivity;
            var integrityBucket = paragraph.CurrentIntegrity / 10;
            var exposureReduction = Mathf.Max(0, baseSensitivity - paragraph.CurrentExposure);
            var isFullMask = paragraph.CurrentActionType == ParagraphActionType.FullMask;

            var informationValueScore = value * integrityBucket * profile.InformationValueWeight;
            var integrityScore = value * integrityBucket * profile.IntegrityWeight;
            var exposureScore = value * paragraph.CurrentExposure * profile.ExposureWeight;
            var exposureReductionScore = value * exposureReduction * profile.ExposureReductionWeight;
            var distortionPenalty = paragraph.CurrentDistortionPenalty * profile.DistortionPenaltyWeight;
            var fullMaskPenalty = isFullMask ? value * 10 * profile.FullMaskPenaltyWeight : 0;

            return new ParagraphScoreBreakdown(
                definition.Id,
                paragraph.CurrentActionType,
                informationValueScore,
                integrityScore,
                exposureScore,
                exposureReductionScore,
                distortionPenalty,
                fullMaskPenalty);
        }
    }
}
