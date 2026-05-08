using App.Gameplay.Runtime;
using App.Gameplay.Loop;

namespace App.Gameplay.Session
{
    public sealed class GameSession
    {
        public GameSession(GameRuntimeState runtimeState, WeekLoop weekLoop)
        {
            RuntimeState = runtimeState;
            WeekLoop = weekLoop;
        }

        public GameRuntimeState RuntimeState { get; }
        public WeekLoop WeekLoop { get; }
    }
}
