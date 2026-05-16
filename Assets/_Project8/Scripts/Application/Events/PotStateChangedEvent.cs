using Project8.Domain.Model;

namespace Project8.Application.Events
{
    public readonly struct PotStateChangedEvent : IGameEvent
    {
        public readonly PotRuntimeModel Pot;

        public PotStateChangedEvent(PotRuntimeModel pot)
        {
            Pot = pot;
        }
    }
}
