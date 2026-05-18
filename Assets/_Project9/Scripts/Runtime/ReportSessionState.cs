using System;
using System.Collections.Generic;
using Project9.Data;

namespace Project9.Runtime
{
    public sealed class ReportSessionState
    {
        private readonly Dictionary<string, ParagraphRuntimeState> _paragraphsById = new();
        private readonly Dictionary<string, ReputationState> _reputationsByTargetId = new();
        private readonly HashSet<string> _submittedTargetIds = new();
        private readonly List<ParagraphRuntimeState> _paragraphs = new();
        private readonly List<ReputationState> _reputations = new();

        public ReportSessionState(
            ReportDefinition reportDefinition,
            IReadOnlyList<SubmitTargetDefinition> submitTargets)
        {
            ReportDefinition = reportDefinition ?? throw new ArgumentNullException(nameof(reportDefinition));

            BuildParagraphStates(reportDefinition);
            BuildReputationStates(submitTargets);
        }

        public ReportDefinition ReportDefinition { get; }
        public SubmitTargetDefinition SelectedSubmitTarget { get; private set; }
        public bool HasSubmitted => _submittedTargetIds.Count > 0;
        public bool AreAllTargetsSubmitted => _reputations.Count > 0 && _submittedTargetIds.Count >= _reputations.Count;
        public int SubmittedTargetCount => _submittedTargetIds.Count;
        public int SubmitTargetCount => _reputations.Count;
        public IReadOnlyList<ParagraphRuntimeState> Paragraphs => _paragraphs;
        public IReadOnlyList<ReputationState> Reputations => _reputations;

        public ParagraphRuntimeState GetParagraph(string paragraphId)
        {
            if (string.IsNullOrWhiteSpace(paragraphId))
            {
                return null;
            }

            _paragraphsById.TryGetValue(paragraphId, out var paragraph);
            return paragraph;
        }

        public ReputationState GetReputation(string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
            {
                return null;
            }

            _reputationsByTargetId.TryGetValue(targetId, out var reputation);
            return reputation;
        }

        public bool SelectParagraphEditOption(string paragraphId, string editOptionId)
        {
            var paragraph = GetParagraph(paragraphId);
            return paragraph != null && paragraph.SelectEditOption(editOptionId);
        }

        public bool SelectSubmitTarget(string targetId)
        {
            var reputation = GetReputation(targetId);
            if (reputation == null)
            {
                return false;
            }

            SelectedSubmitTarget = reputation.TargetDefinition;
            return true;
        }

        public bool HasSubmittedTarget(string targetId)
        {
            return !string.IsNullOrWhiteSpace(targetId) && _submittedTargetIds.Contains(targetId);
        }

        public bool MarkSubmitted(string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId) || !_reputationsByTargetId.ContainsKey(targetId))
            {
                return false;
            }

            return _submittedTargetIds.Add(targetId);
        }

        public void ResetEdits()
        {
            foreach (var paragraph in _paragraphs)
            {
                paragraph.ClearEditOption();
            }
        }

        public void ResetReputations()
        {
            _submittedTargetIds.Clear();

            foreach (var reputation in _reputations)
            {
                reputation.Reset();
            }
        }

        private void BuildParagraphStates(ReportDefinition reportDefinition)
        {
            foreach (var paragraphDefinition in reportDefinition.Paragraphs)
            {
                if (paragraphDefinition == null || string.IsNullOrWhiteSpace(paragraphDefinition.Id))
                {
                    continue;
                }

                if (_paragraphsById.ContainsKey(paragraphDefinition.Id))
                {
                    throw new InvalidOperationException($"Duplicated paragraph id: {paragraphDefinition.Id}");
                }

                var paragraphState = new ParagraphRuntimeState(paragraphDefinition);
                _paragraphs.Add(paragraphState);
                _paragraphsById.Add(paragraphDefinition.Id, paragraphState);
            }
        }

        private void BuildReputationStates(IReadOnlyList<SubmitTargetDefinition> submitTargets)
        {
            foreach (var submitTarget in submitTargets ?? Array.Empty<SubmitTargetDefinition>())
            {
                if (submitTarget == null || string.IsNullOrWhiteSpace(submitTarget.Id))
                {
                    continue;
                }

                if (_reputationsByTargetId.ContainsKey(submitTarget.Id))
                {
                    throw new InvalidOperationException($"Duplicated submit target id: {submitTarget.Id}");
                }

                var reputationState = new ReputationState(submitTarget);
                _reputations.Add(reputationState);
                _reputationsByTargetId.Add(submitTarget.Id, reputationState);
            }
        }
    }
}
