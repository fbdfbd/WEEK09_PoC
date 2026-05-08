using App.Gameplay.Conditions;
using UnityEngine;

namespace App.Gameplay.Definitions
{
    [CreateAssetMenu(fileName = "EndingDefinition_", menuName = "App/Data/Ending Definition")]
    public sealed class EndingDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _priority;
        [SerializeField] private string _title;
        [SerializeField, TextArea(3, 8)] private string _body;
        [SerializeField] private bool _isFallback;
        [SerializeField] private ConditionDefinition _conditions;

        public string Id => _id;
        public int Priority => _priority;
        public string Title => _title;
        public string Body => _body;
        public bool IsFallback => _isFallback;
        public ConditionDefinition Conditions => _conditions;
    }
}
