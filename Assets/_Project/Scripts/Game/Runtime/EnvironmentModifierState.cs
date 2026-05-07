namespace App.Gameplay.Runtime
{
    public sealed class EnvironmentModifierState
    {
        public EnvironmentModifierState(string targetId, int value)
        {
            TargetId = targetId;
            Value = value;
        }

        public string TargetId { get; }
        public int Value { get; set; }
    }
}
