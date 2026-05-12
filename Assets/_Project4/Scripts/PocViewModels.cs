using System;

namespace Project4.NurturePoc
{
    public readonly struct PocStatViewModel
    {
        public PocStatViewModel(string id, string displayName, int value, int minValue, int maxValue)
        {
            Id = id;
            DisplayName = displayName;
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public int Value { get; }
        public int MinValue { get; }
        public int MaxValue { get; }

        public static PocStatViewModel From(PocStatRuntime stat)
        {
            return new PocStatViewModel(stat.Id, stat.DisplayName, stat.Value, stat.MinValue, stat.MaxValue);
        }
    }

    public readonly struct PocChoiceViewModel
    {
        public PocChoiceViewModel(string label, PocActionType actionType)
        {
            Label = label;
            ActionType = actionType;
        }

        public string Label { get; }
        public PocActionType ActionType { get; }
    }

    public sealed class PocLobbyViewModel
    {
        public PocLobbyViewModel(
            int turnIndex,
            string issueTitle,
            string issueBody,
            PocStatViewModel[] stats,
            string statusBody,
            string summary,
            string[] traitTags,
            string[] visibleFlags,
            PocChoiceViewModel[] choices)
        {
            TurnIndex = turnIndex;
            IssueTitle = issueTitle;
            IssueBody = issueBody;
            Stats = stats ?? Array.Empty<PocStatViewModel>();
            StatusBody = statusBody;
            Summary = summary;
            TraitTags = traitTags ?? Array.Empty<string>();
            VisibleFlags = visibleFlags ?? Array.Empty<string>();
            Choices = choices ?? Array.Empty<PocChoiceViewModel>();
        }

        public int TurnIndex { get; }
        public string IssueTitle { get; }
        public string IssueBody { get; }
        public PocStatViewModel[] Stats { get; }
        public string StatusBody { get; }
        public string Summary { get; }
        public string[] TraitTags { get; }
        public string[] VisibleFlags { get; }
        public PocChoiceViewModel[] Choices { get; }

        public static PocLobbyViewModel Empty { get; } = new(
            1,
            string.Empty,
            string.Empty,
            Array.Empty<PocStatViewModel>(),
            string.Empty,
            string.Empty,
            Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<PocChoiceViewModel>());
    }

    public readonly struct PocProgressViewModel
    {
        public PocProgressViewModel(string title, string body)
        {
            Title = title;
            Body = body;
        }

        public string Title { get; }
        public string Body { get; }

        public static PocProgressViewModel Empty { get; } = new(string.Empty, string.Empty);
    }

    public sealed class PocNightDialogueViewModel
    {
        public PocNightDialogueViewModel(string speaker, string body, string[] choiceLabels)
        {
            Speaker = speaker;
            Body = body;
            ChoiceLabels = choiceLabels ?? Array.Empty<string>();
        }

        public string Speaker { get; }
        public string Body { get; }
        public string[] ChoiceLabels { get; }

        public static PocNightDialogueViewModel Empty { get; } = new(string.Empty, string.Empty, Array.Empty<string>());

        public static PocNightDialogueViewModel From(PocNightDialogueDefinition definition)
        {
            if (definition == null)
            {
                return Empty;
            }

            var choices = definition.Choices ?? Array.Empty<PocNightChoiceDefinition>();
            var labels = new string[choices.Length];
            for (var i = 0; i < choices.Length; i++)
            {
                labels[i] = choices[i]?.Label ?? string.Empty;
            }

            return new PocNightDialogueViewModel(definition.Speaker, definition.Body, labels);
        }
    }

    public readonly struct PocNightFeedbackViewModel
    {
        public PocNightFeedbackViewModel(string body)
        {
            Body = body;
        }

        public string Body { get; }

        public static PocNightFeedbackViewModel Empty { get; } = new(string.Empty);
    }
}
