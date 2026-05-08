using System.Collections.Generic;
using App.Gameplay.Reports;

namespace App.Gameplay.Runtime
{
    public sealed class GameRuntimeState
    {
        private readonly HashSet<string> _flags = new();
        private readonly List<ReportEntry> _reports = new();

        public int CurrentWeekIndex { get; private set; } = 1;
        public bool HasReachedEnding { get; private set; }
        public CharacterState Character { get; } = new();
        public EnvironmentState Environment { get; } = new();
        public IReadOnlyCollection<string> Flags => _flags;
        public IReadOnlyList<ReportEntry> Reports => _reports;

        public bool HasFlag(string flagId)
        {
            return !string.IsNullOrWhiteSpace(flagId) && _flags.Contains(flagId);
        }

        public void AddFlag(string flagId)
        {
            if (!string.IsNullOrWhiteSpace(flagId))
            {
                _flags.Add(flagId);
            }
        }

        public void RemoveFlag(string flagId)
        {
            if (!string.IsNullOrWhiteSpace(flagId))
            {
                _flags.Remove(flagId);
            }
        }

        public void AddReport(ReportEntry report)
        {
            if (report != null)
            {
                _reports.Add(report);
            }
        }

        public void MoveToNextWeek()
        {
            CurrentWeekIndex++;
        }

        public void MarkEndingReached()
        {
            HasReachedEnding = true;
        }
    }
}
