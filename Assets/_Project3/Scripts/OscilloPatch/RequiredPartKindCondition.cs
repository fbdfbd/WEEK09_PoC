using System.Collections.Generic;

namespace Project3.OscilloPatch
{
    public sealed class RequiredPartKindCondition : MissionCondition
    {
        private readonly SignalPartKind kind;
        private readonly string label;

        public string Description => label;

        public RequiredPartKindCondition(SignalPartKind kind, string label)
        {
            this.kind = kind;
            this.label = label;
        }

        public bool IsMet(SignalPair signals, IReadOnlyList<UnityEngine.Vector2> points)
        {
            return signals.IsValid && signals.HasPartKind(kind);
        }
    }
}
