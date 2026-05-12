public class OrderSystem
{
    public bool IsPlayerFirst(BattleState state)
    {
        return state.GetOrderNumberValue() >= state.EnemyOrderValue;
    }
}
