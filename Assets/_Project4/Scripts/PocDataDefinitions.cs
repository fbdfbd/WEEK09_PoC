using System;
using UnityEngine;

namespace Project4.NurturePoc
{
    public enum PocActionType
    {
        Allow,
        Block,
        Control,
        Custom
    }

    public enum PocEffectType
    {
        ChangeStat,
        SetStat,
        AddFlag,
        RemoveFlag
    }

    public enum PocScreen
    {
        Lobby,
        Progress,
        NightDialogue,
        NightFeedback,
        Finished
    }

    [CreateAssetMenu(fileName = "PocStat_", menuName = "Project4/Poc/Stat")]
    public sealed class PocStatDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private bool _visibleInLobby = true;
        [SerializeField] private int _defaultValue = 50;
        [SerializeField] private int _minValue;
        [SerializeField] private int _maxValue = 100;

        public string Id => _id;
        public string DisplayName => _displayName;
        public bool VisibleInLobby => _visibleInLobby;
        public int DefaultValue => _defaultValue;
        public int MinValue => _minValue;
        public int MaxValue => _maxValue;
    }

    [CreateAssetMenu(fileName = "PocFlag_", menuName = "Project4/Poc/Flag")]
    public sealed class PocFlagDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private bool _visibleInLobby;
        [SerializeField] private int _priority;

        public string Id => _id;
        public string DisplayName => _displayName;
        public bool VisibleInLobby => _visibleInLobby;
        public int Priority => _priority;
    }

    [CreateAssetMenu(fileName = "PocLobbyStatus_", menuName = "Project4/Poc/Lobby Status")]
    public sealed class PocLobbyStatusDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _priority;
        [SerializeField] private PocCondition _condition = new();
        [SerializeField, TextArea(2, 6)] private string _statusBody;
        [SerializeField] private string _summary;
        [SerializeField] private string[] _traitTags = Array.Empty<string>();

        public string Id => _id;
        public int Priority => _priority;
        public PocCondition Condition => _condition;
        public string StatusBody => _statusBody;
        public string Summary => _summary;
        public string[] TraitTags => _traitTags;
    }

    [CreateAssetMenu(fileName = "PocTurnIssue_", menuName = "Project4/Poc/Turn Issue")]
    public sealed class PocTurnIssueDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _turnIndex = 1;
        [SerializeField] private int _priority;
        [SerializeField] private PocCondition _condition = new();
        [SerializeField] private string _title;
        [SerializeField, TextArea(2, 6)] private string _body;
        [SerializeField] private PocChoiceDefinition[] _choices = Array.Empty<PocChoiceDefinition>();

        public string Id => _id;
        public int TurnIndex => _turnIndex;
        public int Priority => _priority;
        public PocCondition Condition => _condition;
        public string Title => _title;
        public string Body => _body;
        public PocChoiceDefinition[] Choices => _choices;
    }

    [CreateAssetMenu(fileName = "PocProgress_", menuName = "Project4/Poc/Progress")]
    public sealed class PocProgressDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _turnIndex = 1;
        [SerializeField] private int _priority;
        [SerializeField] private PocCondition _condition = new();
        [SerializeField] private string _title;
        [SerializeField, TextArea(3, 8)] private string _body;
        [SerializeField] private PocEffect[] _effects = Array.Empty<PocEffect>();

        public string Id => _id;
        public int TurnIndex => _turnIndex;
        public int Priority => _priority;
        public PocCondition Condition => _condition;
        public string Title => _title;
        public string Body => _body;
        public PocEffect[] Effects => _effects;
    }

    [CreateAssetMenu(fileName = "PocNightDialogue_", menuName = "Project4/Poc/Night Dialogue")]
    public sealed class PocNightDialogueDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _turnIndex = 1;
        [SerializeField] private int _priority;
        [SerializeField] private PocCondition _condition = new();
        [SerializeField] private string _speaker = "Nemo";
        [SerializeField, TextArea(2, 6)] private string _body;
        [SerializeField] private PocNightChoiceDefinition[] _choices = Array.Empty<PocNightChoiceDefinition>();

        public string Id => _id;
        public int TurnIndex => _turnIndex;
        public int Priority => _priority;
        public PocCondition Condition => _condition;
        public string Speaker => _speaker;
        public string Body => _body;
        public PocNightChoiceDefinition[] Choices => _choices;
    }

    [Serializable]
    public sealed class PocChoiceDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private string _label;
        [SerializeField] private PocActionType _actionType;
        [SerializeField] private string _categoryId = "place";
        [SerializeField] private string _targetId = "lobby";
        [SerializeField] private PocCondition _condition = new();
        [SerializeField] private PocEffect[] _effects = Array.Empty<PocEffect>();

        public string Id => _id;
        public string Label => _label;
        public PocActionType ActionType => _actionType;
        public string CategoryId => _categoryId;
        public string TargetId => _targetId;
        public PocCondition Condition => _condition;
        public PocEffect[] Effects => _effects;
    }

    [Serializable]
    public sealed class PocNightChoiceDefinition
    {
        [SerializeField] private string _label;
        [SerializeField, TextArea(2, 5)] private string _feedback;
        [SerializeField] private PocEffect[] _effects = Array.Empty<PocEffect>();

        public string Label => _label;
        public string Feedback => _feedback;
        public PocEffect[] Effects => _effects;
    }

    [Serializable]
    public sealed class PocCondition
    {
        [SerializeField] private string[] _requiredFlags = Array.Empty<string>();
        [SerializeField] private string[] _blockedFlags = Array.Empty<string>();
        [SerializeField] private PocStatRequirement[] _statRequirements = Array.Empty<PocStatRequirement>();

        public string[] RequiredFlags => _requiredFlags;
        public string[] BlockedFlags => _blockedFlags;
        public PocStatRequirement[] StatRequirements => _statRequirements;
    }

    [Serializable]
    public sealed class PocStatRequirement
    {
        [SerializeField] private string _statId;
        [SerializeField] private bool _useMinimum;
        [SerializeField] private int _minimum;
        [SerializeField] private bool _useMaximum;
        [SerializeField] private int _maximum = 100;

        public string StatId => _statId;
        public bool UseMinimum => _useMinimum;
        public int Minimum => _minimum;
        public bool UseMaximum => _useMaximum;
        public int Maximum => _maximum;
    }

    [Serializable]
    public sealed class PocEffect
    {
        [SerializeField] private PocEffectType _type;
        [SerializeField] private string _targetId;
        [SerializeField] private int _value;

        public PocEffectType Type => _type;
        public string TargetId => _targetId;
        public int Value => _value;
    }
}
