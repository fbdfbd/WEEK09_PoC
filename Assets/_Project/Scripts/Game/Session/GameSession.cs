using App.Gameplay.Runtime;

namespace App.Gameplay.Session
{
    public sealed class GameSession
    {
        public GameSession(GameRuntimeState runtimeState)
        {
            RuntimeState = runtimeState;
        }

        public GameRuntimeState RuntimeState { get; }
    }
}
