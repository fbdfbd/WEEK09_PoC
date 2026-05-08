using System;
using App.Gameplay.Conditions;
using App.Gameplay.Effects;
using UnityEngine;

namespace App.Gameplay.Definitions
{
    [CreateAssetMenu(fileName = "NoonAction_", menuName = "App/Data/Noon Action")]
    public sealed class NoonActionDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _priority;
        [SerializeField] private string _title;
        [SerializeField, TextArea(2, 6)] private string _reportBody;
        [SerializeField] private ConditionDefinition _conditions;
        [SerializeField] private EffectDefinition[] _effects = Array.Empty<EffectDefinition>();

        public string Id => _id;
        public int Priority => _priority;
        public string Title => _title;
        public string ReportBody => _reportBody;
        public ConditionDefinition Conditions => _conditions;
        public EffectDefinition[] Effects => _effects;
    }
}
