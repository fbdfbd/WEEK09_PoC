using System;
using App.Gameplay.Conditions;
using App.Gameplay.Effects;
using App.Gameplay.Environment;
using UnityEngine;

namespace App.Gameplay.Definitions
{
    [CreateAssetMenu(fileName = "MorningIssue_", menuName = "App/Data/Morning Issue")]
    public sealed class MorningIssueDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _priority;
        [SerializeField] private string _title;
        [SerializeField, TextArea(2, 6)] private string _body;
        [SerializeField] private ConditionDefinition _conditions;
        [SerializeField] private MorningOptionDefinition[] _options = Array.Empty<MorningOptionDefinition>();

        public string Id => _id;
        public int Priority => _priority;
        public string Title => _title;
        public string Body => _body;
        public ConditionDefinition Conditions => _conditions;
        public MorningOptionDefinition[] Options => _options;
    }

    [Serializable]
    public sealed class MorningOptionDefinition
    {
        [SerializeField] private string _label;
        [SerializeField] private EnvironmentControlType _controlType;
        [SerializeField] private EffectDefinition[] _effects = Array.Empty<EffectDefinition>();

        public string Label => _label;
        public EnvironmentControlType ControlType => _controlType;
        public EffectDefinition[] Effects => _effects;
    }
}
