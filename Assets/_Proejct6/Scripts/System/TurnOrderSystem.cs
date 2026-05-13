public class TurnOrderSystem
{
    public bool IsPlayerFirst(BattleState state)
    {
        return state.GetOrderValue() >= state.GetEnemyOrderValue();
    }
}
