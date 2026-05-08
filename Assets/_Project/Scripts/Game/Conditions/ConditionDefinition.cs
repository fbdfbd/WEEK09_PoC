using System;
using App.Gameplay.Environment;
using App.Gameplay.Runtime;
using UnityEngine;

namespace App.Gameplay.Conditions
{
    [Serializable]
    public sealed class ConditionDefinition
    {
        [SerializeField] private string[] _requiredFlags = Array.Empty<string>();
        [SerializeField] private string[] _blockedFlags = Array.Empty<string>();
        [SerializeField] private StatRequirement[] _statRequirements = Array.Empty<StatRequirement>();
        [SerializeField] private EnvironmentRequirement[] _environmentRequirements = Array.Empty<EnvironmentRequirement>();

        public string[] RequiredFlags => _requiredFlags;
        public string[] BlockedFlags => _blockedFlags;
        public StatRequirement[] StatRequirements => _statRequirements;
        public EnvironmentRequirement[] EnvironmentRequirements => _environmentRequirements;
    }

    [Serializable]
    public sealed class StatRequirement
    {
        [SerializeField] private CharacterStatType _statType;
        [SerializeField] private bool _useMinimum;
        [SerializeField] private int _minimum;
        [SerializeField] private bool _useMaximum;
        [SerializeField] private int _maximum = CharacterState.MaxStatValue;

        public CharacterStatType StatType => _statType;
        public bool UseMinimum => _useMinimum;
        public int Minimum => _minimum;
        public bool UseMaximum => _useMaximum;
        public int Maximum => _maximum;
    }

    [Serializable]
    public sealed class EnvironmentRequirement
    {
        [SerializeField] private EnvironmentControlType _type;
        [SerializeField] private string _targetId;

        public EnvironmentControlType Type => _type;
        public string TargetId => _targetId;
    }
}
