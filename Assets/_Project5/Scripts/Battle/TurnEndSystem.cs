public class TurnEndSystem
{
    public void EndTurn(BattleState state, EnemySystem enemySystem)
    {
        MoveUsedOrderNumber(state);
        MoveUsedSkillSlots(state);
        enemySystem.MoveEnemyUsedCardsToDiscard(state);

        state.LastTurnUsedNumberCount = state.UsedNumbersThisTurn.Count;
        state.EnemyLastTurnUsedNumberCount = state.EnemyUsedNumbersThisTurn.Count;
        state.UsedNumbersThisTurn.Clear();
        state.EnemyUsedNumbersThisTurn.Clear();
        state.UsedNumberCardsThisTurn.Clear();
        state.EnemyUsedNumberCardsThisTurn.Clear();
        state.UsedSkillCardsThisTurn.Clear();
        state.EnemyUsedSkillCardsThisTurn.Clear();
        state.SkillSlots.Clear();
        state.EnemySkillSlots.Clear();
        state.OrderSlotNumber = null;
        state.IsFirstTurn = false;
        state.TurnNumber++;
    }

    private void MoveUsedOrderNumber(BattleState state)
    {
        if (state.OrderSlotNumber == null)
        {
            return;
        }

        state.NumberDiscard.Add(state.OrderSlotNumber);
    }

    private void MoveUsedSkillSlots(BattleState state)
    {
        for (int i = 0; i < state.SkillSlots.Count; i++)
        {
            SkillSlotState slot = state.SkillSlots[i];

            for (int numberIndex = 0; numberIndex < slot.NumberCards.Count; numberIndex++)
            {
                state.NumberDiscard.Add(slot.NumberCards[numberIndex]);
            }

            state.SkillDiscard.Add(slot.SkillCard);
        }
    }
}
