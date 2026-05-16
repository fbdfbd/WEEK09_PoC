using System;
using Project9.Data;

namespace Project9.Runtime
{
    public sealed class ReputationState
    {
        public ReputationState(SubmitTargetDefinition targetDefinition)
        {
            TargetDefinition = targetDefinition ?? throw new ArgumentNullException(nameof(targetDefinition));
            Value = targetDefinition.InitialReputation;
        }

        public SubmitTargetDefinition TargetDefinition { get; }
        public int Value { get; private set; }

        public void Add(int delta)
        {
            Value += delta;
        }

        public void Set(int value)
        {
            Value = value;
        }

        public void Reset()
        {
            Value = TargetDefinition.InitialReputation;
        }
    }
}
