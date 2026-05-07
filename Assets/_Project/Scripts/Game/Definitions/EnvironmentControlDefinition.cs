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
        [SerializeField] private string id;
        [SerializeField] private EnvironmentControlType type;
        [SerializeField] private string displayNameKey;
        [SerializeField] private string descriptionKey;
        [SerializeField] private EffectDefinition[] effects;

        public string Id => id;
        public EnvironmentControlType Type => type;
        public string DisplayNameKey => displayNameKey;
        public string DescriptionKey => descriptionKey;
        public EffectDefinition[] Effects => effects;
    }
}
