using App.Gameplay.Effects;
using App.Gameplay.Environment;
using UnityEngine;

namespace App.Gameplay.Definitions
{
    [CreateAssetMenu(
        fileName = "EnvironmentControlDefinition",
        menuName = "App/Data/Environment Control")]
    public sealed class EnvironmentControlDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private EnvironmentControlType _type;
        [SerializeField] private string _displayNameKey;
        [SerializeField] private string _descriptionKey;
        [SerializeField] private EffectDefinition[] _effects;

        public string Id => _id;
        public EnvironmentControlType Type => _type;
        public string DisplayNameKey => _displayNameKey;
        public string DescriptionKey => _descriptionKey;
        public EffectDefinition[] Effects => _effects;
    }
}
