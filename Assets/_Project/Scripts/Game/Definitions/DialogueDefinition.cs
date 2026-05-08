using System;
using App.Gameplay.Conditions;
using App.Gameplay.Effects;
using UnityEngine;

namespace App.Gameplay.Definitions
{
    [CreateAssetMenu(fileName = "Dialogue_", menuName = "App/Data/Dialogue")]
    public sealed class DialogueDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _priority;
        [SerializeField] private string _title;
        [SerializeField] private ConditionDefinition _conditions;
        [SerializeField] private string _firstNodeId;
        [SerializeField] private DialogueNodeDefinition[] _nodes = Array.Empty<DialogueNodeDefinition>();

        public string Id => _id;
        public int Priority => _priority;
        public string Title => _title;
        public ConditionDefinition Conditions => _conditions;
        public string FirstNodeId => _firstNodeId;
        public DialogueNodeDefinition[] Nodes => _nodes;

        public DialogueNodeDefinition GetNode(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId) || _nodes == null)
            {
                return null;
            }

            for (var i = 0; i < _nodes.Length; i++)
            {
                var node = _nodes[i];
                if (node != null && node.Id == nodeId)
                {
                    return node;
                }
            }

            return null;
        }
    }

    [Serializable]
    public sealed class DialogueNodeDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private string _speaker;
        [SerializeField, TextArea(2, 6)] private string _body;
        [SerializeField] private DialogueChoiceDefinition[] _choices = Array.Empty<DialogueChoiceDefinition>();

        public string Id => _id;
        public string Speaker => _speaker;
        public string Body => _body;
        public DialogueChoiceDefinition[] Choices => _choices;
    }

    [Serializable]
    public sealed class DialogueChoiceDefinition
    {
        [SerializeField] private string _label;
        [SerializeField] private EffectDefinition[] _effects = Array.Empty<EffectDefinition>();
        [SerializeField] private string _nextNodeId;

        public string Label => _label;
        public EffectDefinition[] Effects => _effects;
        public string NextNodeId => _nextNodeId;
    }
}
