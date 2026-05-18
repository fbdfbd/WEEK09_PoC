using System;
using System.Collections.Generic;
using Project9.Data;
using Project9.Runtime;
using Project9.Systems;
using R3;

namespace Project9.Presentation
{
    public sealed class Project9Presenter : IDisposable
    {
        private readonly ReportSessionFactory _sessionFactory;
        private readonly SubmissionScoringSystem _scoringSystem;
        private readonly ReputationSystem _reputationSystem;
        private readonly Subject<ReportViewModel> _reportChanged = new();
        private readonly Subject<SubmissionResultViewModel> _submissionCompleted = new();
        private readonly Subject<FinalResultViewModel> _finalResultCompleted = new();
        private readonly Dictionary<string, SubmissionResultViewModel> _submissionsByTargetId = new();
        private Project9ScenarioDefinition _scenario;
        private ReportSessionState _session;

        public Project9Presenter(
            ReportSessionFactory sessionFactory,
            SubmissionScoringSystem scoringSystem,
            ReputationSystem reputationSystem)
        {
            _sessionFactory = sessionFactory;
            _scoringSystem = scoringSystem;
            _reputationSystem = reputationSystem;
        }

        public Observable<ReportViewModel> ReportChanged => _reportChanged;
        public Observable<SubmissionResultViewModel> SubmissionCompleted => _submissionCompleted;
        public Observable<FinalResultViewModel> FinalResultCompleted => _finalResultCompleted;
        public ReportSessionState Session => _session;
        public ReportViewModel CurrentReport { get; private set; }
        public SubmissionResultViewModel LastSubmissionResult { get; private set; }
        public FinalResultViewModel CurrentFinalResult { get; private set; }

        public void Initialize(Project9ScenarioDefinition scenario)
        {
            if (scenario == null)
            {
                throw new ArgumentNullException(nameof(scenario));
            }

            _scenario = scenario;
            _session = _sessionFactory.Create(scenario.Report, scenario.SubmitTargets);
            _submissionsByTargetId.Clear();
            LastSubmissionResult = null;
            CurrentFinalResult = null;

            if (scenario.SubmitTargets.Count > 0 && scenario.SubmitTargets[0] != null)
            {
                _session.SelectSubmitTarget(scenario.SubmitTargets[0].Id);
            }

            PublishReport();
        }

        public void Restart()
        {
            EnsureInitialized();
            Initialize(_scenario);
        }

        public bool SelectParagraphEditOption(string paragraphId, string editOptionId)
        {
            EnsureInitialized();

            var changed = _session.SelectParagraphEditOption(paragraphId, editOptionId);
            if (changed)
            {
                PublishReport();
            }

            return changed;
        }

        public bool SelectSubmitTarget(string targetId)
        {
            EnsureInitialized();

            var changed = _session.SelectSubmitTarget(targetId);
            if (changed)
            {
                PublishReport();
            }

            return changed;
        }

        public SubmissionResultViewModel PreviewSubmission(string targetId)
        {
            EnsureInitialized();

            var reputation = _session.GetReputation(targetId);
            if (reputation == null)
            {
                return null;
            }

            return BuildSubmissionResultViewModel(
                _scoringSystem.Calculate(_session, reputation.TargetDefinition));
        }

        public SubmissionResultViewModel SubmitSelectedTarget()
        {
            EnsureInitialized();

            var selectedTargetId = _session.SelectedSubmitTarget?.Id;
            if (_session.HasSubmittedTarget(selectedTargetId))
            {
                return _submissionsByTargetId.TryGetValue(selectedTargetId, out var previousResult)
                    ? previousResult
                    : LastSubmissionResult;
            }

            var result = _scoringSystem.CalculateSelectedTarget(_session);
            _reputationSystem.ApplySubmissionResult(_session, result);
            _session.MarkSubmitted(result.Target.Id);

            var viewModel = BuildSubmissionResultViewModel(result);
            LastSubmissionResult = viewModel;
            _submissionsByTargetId[result.Target.Id] = viewModel;
            _submissionCompleted.OnNext(viewModel);
            PublishReport();

            if (_session.AreAllTargetsSubmitted)
            {
                CurrentFinalResult = BuildFinalResultViewModel();
                _finalResultCompleted.OnNext(CurrentFinalResult);
            }

            return viewModel;
        }

        public void Dispose()
        {
            _reportChanged.OnCompleted();
            _submissionCompleted.OnCompleted();
            _finalResultCompleted.OnCompleted();
            _reportChanged.Dispose();
            _submissionCompleted.Dispose();
            _finalResultCompleted.Dispose();
        }

        private void PublishReport()
        {
            CurrentReport = BuildReportViewModel();
            _reportChanged.OnNext(CurrentReport);
        }

        private ReportViewModel BuildReportViewModel()
        {
            var paragraphs = new List<ParagraphViewModel>();
            foreach (var paragraph in _session.Paragraphs)
            {
                paragraphs.Add(new ParagraphViewModel(
                    paragraph.Definition.Id,
                    paragraph.Definition.Title,
                    paragraph.CurrentText,
                    paragraph.Definition.InformationValue,
                    paragraph.Definition.Sensitivity,
                    paragraph.CurrentIntegrity,
                    paragraph.CurrentExposure,
                    paragraph.CurrentActionType,
                    paragraph.SelectedEditOption?.Id));
            }

            var targets = new List<TargetViewModel>();
            foreach (var reputation in _session.Reputations)
            {
                var target = reputation.TargetDefinition;
                targets.Add(new TargetViewModel(
                    target.Id,
                    target.DisplayName,
                    target.Description,
                    reputation.Value,
                    string.Equals(_session.SelectedSubmitTarget?.Id, target.Id, StringComparison.Ordinal),
                    _session.HasSubmittedTarget(target.Id)));
            }

            var selectedTargetId = _session.SelectedSubmitTarget?.Id;
            return new ReportViewModel(
                _session.ReportDefinition.Title,
                _session.ReportDefinition.Summary,
                paragraphs,
                targets,
                selectedTargetId,
                _session.HasSubmittedTarget(selectedTargetId));
        }

        private static SubmissionResultViewModel BuildSubmissionResultViewModel(SubmissionResult result)
        {
            var paragraphScores = new List<ParagraphScoreViewModel>();
            foreach (var breakdown in result.ParagraphBreakdowns)
            {
                paragraphScores.Add(new ParagraphScoreViewModel(
                    breakdown.ParagraphId,
                    breakdown.ActionType,
                    breakdown.Total,
                    breakdown.InformationValueScore,
                    breakdown.IntegrityScore,
                    breakdown.ExposureScore,
                    breakdown.ExposureReductionScore,
                    breakdown.DistortionPenalty,
                    breakdown.FullMaskPenalty));
            }

            return new SubmissionResultViewModel(
                result.Target.Id,
                result.Target.DisplayName,
                result.TotalScore,
                result.ReputationDelta,
                paragraphScores);
        }

        private FinalResultViewModel BuildFinalResultViewModel()
        {
            var targetReputations = new List<FinalTargetReputationViewModel>();
            var finalReputation = 0;
            var maxReputation = 0;
            var hasAnyTarget = false;

            foreach (var reputation in _session.Reputations)
            {
                var target = reputation.TargetDefinition;
                var value = reputation.Value;
                targetReputations.Add(new FinalTargetReputationViewModel(
                    target.Id,
                    target.DisplayName,
                    value));

                finalReputation += value;

                if (!hasAnyTarget || value > maxReputation)
                {
                    maxReputation = value;
                    hasAnyTarget = true;
                }
            }

            return new FinalResultViewModel(
                finalReputation,
                maxReputation,
                targetReputations);
        }

        private void EnsureInitialized()
        {
            if (_session == null)
            {
                throw new InvalidOperationException("Project9Presenter is not initialized.");
            }
        }
    }
}
