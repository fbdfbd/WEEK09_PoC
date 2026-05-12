public class BattleContext
{
    private readonly BattleState state;

    public BattleState State
    {
        get { return state; }
    }

    public BattleActorState Player
    {
        get { return state.Player; }
    }

    public BattleActorState Enemy
    {
        get { return state.Enemy; }
    }

    public BattleContext(BattleState state)
    {
        this.state = state;
    }
}
