using System;
using System.Collections.Generic;
using System.Linq;

namespace Project4.NurturePoc
{
    public sealed class PocRuntimeState
    {
        private readonly Dictionary<string, PocStatRuntime> _stats = new();
        private readonly HashSet<string> _flags = new();

        public int CurrentTurn { get; private set; } = 1;
        public IReadOnlyDictionary<string, PocStatRuntime> Stats => _stats;
        public IReadOnlyCollection<string> Flags => _flags;
        public PocChoiceDefinition LastChoice { get; private set; }

        public PocRuntimeState(IEnumerable<PocStatDefinition> statDefinitions)
        {
            foreach (var definition in statDefinitions ?? Array.Empty<PocStatDefinition>())
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.Id))
                {
                    continue;
                }

                _stats[definition.Id] = new PocStatRuntime(definition);
            }
        }

        public int GetStat(string statId)
        {
            return !string.IsNullOrWhiteSpace(statId) && _stats.TryGetValue(statId, out var stat)
                ? stat.Value
                : 0;
        }

        public void SetStat(string statId, int value)
        {
            if (string.IsNullOrWhiteSpace(statId) || !_stats.TryGetValue(statId, out var stat))
            {
                return;
            }

            stat.Value = Math.Clamp(value, stat.MinValue, stat.MaxValue);
        }

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

        public void SetLastChoice(PocChoiceDefinition choice)
        {
            LastChoice = choice;
        }

        public void NextTurn()
        {
            CurrentTurn++;
            LastChoice = null;
        }
    }

    public sealed class PocStatRuntime
    {
        public PocStatRuntime(PocStatDefinition definition)
        {
            Id = definition.Id;
            DisplayName = definition.DisplayName;
            VisibleInLobby = definition.VisibleInLobby;
            MinValue = definition.MinValue;
            MaxValue = definition.MaxValue;
            Value = definition.DefaultValue;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public bool VisibleInLobby { get; }
        public int MinValue { get; }
        public int MaxValue { get; }
        public int Value { get; set; }
    }

    public sealed class PocFlow
    {
        private readonly PocConditionEvaluator _conditions = new();
        private readonly PocEffectProcessor _effects = new();
        private readonly PocStatDefinition[] _stats;
        private readonly PocFlagDefinition[] _flags;
        private readonly PocLobbyStatusDefinition[] _lobbyStatuses;
        private readonly PocTurnIssueDefinition[] _issues;
        private readonly PocProgressDefinition[] _progresses;
        private readonly PocNightDialogueDefinition[] _nightDialogues;

        private PocProgressDefinition _currentProgress;
        private PocNightDialogueDefinition _currentNightDialogue;

        public PocFlow(
            PocStatDefinition[] stats,
            PocFlagDefinition[] flags,
            PocLobbyStatusDefinition[] lobbyStatuses,
            PocTurnIssueDefinition[] issues,
            PocProgressDefinition[] progresses,
            PocNightDialogueDefinition[] nightDialogues)
        {
            _stats = stats ?? Array.Empty<PocStatDefinition>();
            _flags = flags ?? Array.Empty<PocFlagDefinition>();
            _lobbyStatuses = lobbyStatuses ?? Array.Empty<PocLobbyStatusDefinition>();
            _issues = issues ?? Array.Empty<PocTurnIssueDefinition>();
            _progresses = progresses ?? Array.Empty<PocProgressDefinition>();
            _nightDialogues = nightDialogues ?? Array.Empty<PocNightDialogueDefinition>();
            State = new PocRuntimeState(_stats);
        }

        public PocRuntimeState State { get; }

        public PocLobbyViewModel BuildLobby()
        {
            var issue = SelectOne(_issues.Where(x => x.TurnIndex == State.CurrentTurn), x => x.Condition, x => x.Priority);
            var lobbyStatus = SelectOne(_lobbyStatuses, x => x.Condition, x => x.Priority);
            var choices = issue?.Choices?
                .Where(choice => choice != null && _conditions.IsMet(choice.Condition, State))
                .Select(choice => new PocChoiceViewModel(choice.Label, choice.ActionType))
                .ToArray() ?? Array.Empty<PocChoiceViewModel>();

            return new PocLobbyViewModel(
                State.CurrentTurn,
                issue?.Title ?? string.Empty,
                issue?.Body ?? string.Empty,
                State.Stats.Values.Where(x => x.VisibleInLobby).Select(PocStatViewModel.From).ToArray(),
                lobbyStatus?.StatusBody ?? string.Empty,
                lobbyStatus?.Summary ?? string.Empty,
                lobbyStatus?.TraitTags ?? Array.Empty<string>(),
                BuildVisibleFlags(),
                choices);
        }

        public PocProgressViewModel SelectChoice(int choiceIndex)
        {
            var issue = SelectOne(_issues.Where(x => x.TurnIndex == State.CurrentTurn), x => x.Condition, x => x.Priority);
            var choices = issue?.Choices?
                .Where(choice => choice != null && _conditions.IsMet(choice.Condition, State))
                .ToArray() ?? Array.Empty<PocChoiceDefinition>();

            if (choiceIndex < 0 || choiceIndex >= choices.Length)
            {
                return PocProgressViewModel.Empty;
            }

            var choice = choices[choiceIndex];
            State.SetLastChoice(choice);
            _effects.ApplyAll(choice.Effects, State);

            _currentProgress = SelectOne(
                _progresses.Where(x => x.TurnIndex == State.CurrentTurn),
                x => x.Condition,
                x => x.Priority);

            return new PocProgressViewModel(_currentProgress?.Title ?? string.Empty, _currentProgress?.Body ?? string.Empty);
        }

        public PocNightDialogueViewModel CompleteProgress()
        {
            _effects.ApplyAll(_currentProgress?.Effects, State);

            _currentNightDialogue = SelectOne(
                _nightDialogues.Where(x => x.TurnIndex == State.CurrentTurn),
                x => x.Condition,
                x => x.Priority);

            return PocNightDialogueViewModel.From(_currentNightDialogue);
        }

        public PocNightFeedbackViewModel SelectNightChoice(int choiceIndex)
        {
            var choices = _currentNightDialogue?.Choices ?? Array.Empty<PocNightChoiceDefinition>();
            if (choiceIndex < 0 || choiceIndex >= choices.Length)
            {
                return PocNightFeedbackViewModel.Empty;
            }

            var choice = choices[choiceIndex];
            _effects.ApplyAll(choice.Effects, State);
            return new PocNightFeedbackViewModel(choice.Feedback);
        }

        public void CompleteNightFeedback()
        {
            State.NextTurn();
            _currentProgress = null;
            _currentNightDialogue = null;
        }

        public bool HasNextTurn()
        {
            return _issues.Any(x => x != null && x.TurnIndex == State.CurrentTurn);
        }

        private string[] BuildVisibleFlags()
        {
            return _flags
                .Where(x => x != null && x.VisibleInLobby && State.HasFlag(x.Id))
                .OrderByDescending(x => x.Priority)
                .Select(x => x.DisplayName)
                .ToArray();
        }

        private T SelectOne<T>(IEnumerable<T> source, Func<T, PocCondition> condition, Func<T, int> priority)
            where T : class
        {
            return source
                .Where(x => x != null && _conditions.IsMet(condition(x), State))
                .OrderByDescending(priority)
                .FirstOrDefault();
        }
    }

    public sealed class PocConditionEvaluator
    {
        public bool IsMet(PocCondition condition, PocRuntimeState state)
        {
            if (condition == null)
            {
                return true;
            }

            foreach (var flag in condition.RequiredFlags ?? Array.Empty<string>())
            {
                if (!state.HasFlag(flag))
                {
                    return false;
                }
            }

            foreach (var flag in condition.BlockedFlags ?? Array.Empty<string>())
            {
                if (state.HasFlag(flag))
                {
                    return false;
                }
            }

            foreach (var stat in condition.StatRequirements ?? Array.Empty<PocStatRequirement>())
            {
                if (stat == null)
                {
                    continue;
                }

                var value = state.GetStat(stat.StatId);
                if (stat.UseMinimum && value < stat.Minimum)
                {
                    return false;
                }

                if (stat.UseMaximum && value > stat.Maximum)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public sealed class PocEffectProcessor
    {
        public void ApplyAll(IEnumerable<PocEffect> effects, PocRuntimeState state)
        {
            if (effects == null)
            {
                return;
            }

            foreach (var effect in effects)
            {
                Apply(effect, state);
            }
        }

        private static void Apply(PocEffect effect, PocRuntimeState state)
        {
            if (effect == null)
            {
                return;
            }

            switch (effect.Type)
            {
                case PocEffectType.ChangeStat:
                    state.SetStat(effect.TargetId, state.GetStat(effect.TargetId) + effect.Value);
                    break;
                case PocEffectType.SetStat:
                    state.SetStat(effect.TargetId, effect.Value);
                    break;
                case PocEffectType.AddFlag:
                    state.AddFlag(effect.TargetId);
                    break;
                case PocEffectType.RemoveFlag:
                    state.RemoveFlag(effect.TargetId);
                    break;
            }
        }
    }
}
