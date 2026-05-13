public class BattleContext
{
    public BattleState State { get; private set; }
    public BattleActorState Player => State.Player;
    public BattleActorState Enemy => State.Enemy;

    public BattleContext(BattleState state)
    {
        State = state;
    }
}
