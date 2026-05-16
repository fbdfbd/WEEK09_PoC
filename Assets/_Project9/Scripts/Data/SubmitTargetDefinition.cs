using UnityEngine;

namespace Project9.Data
{
    [CreateAssetMenu(menuName = "Project9/Submit Target Definition", fileName = "SubmitTargetDefinition")]
    public sealed class SubmitTargetDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea(2, 6)]
        [SerializeField] private string description;
        [SerializeField] private SubmitTargetKind kind;
        [SerializeField] private int initialReputation;
        [SerializeField] private SubmitTargetScoringProfile scoringProfile = new();

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public SubmitTargetKind Kind => kind;
        public int InitialReputation => initialReputation;
        public SubmitTargetScoringProfile ScoringProfile => scoringProfile;
    }
}
