namespace App.Gameplay.Runtime
{
    public sealed class GameRuntimeState
    {
        public EnvironmentState Environment { get; } = new();
    }
}
