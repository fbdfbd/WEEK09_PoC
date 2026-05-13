public class TurnEndSystem
{
    public void EndTurn(BattleState state)
    {
        MoveOrderNumber(state);
        MoveSkillSlots(state);

        state.LastTurnUsedNumberCount = state.UsedNumbersThisTurn.Count;
        state.UsedNumbersThisTurn.Clear();
        state.UsedNumberCardsThisTurn.Clear();
        state.UsedSkillCardsThisTurn.Clear();
        state.SkillSlots.Clear();
        state.OrderSlotNumber = null;
        state.IsFirstTurn = false;
        state.TurnNumber++;
    }

    private void MoveOrderNumber(BattleState state)
    {
        if (state.OrderSlotNumber == null)
        {
            return;
        }

        NumCardDefinition definition = BattleState.GetNumDefinition(state.OrderSlotNumber);
        if (definition != null)
        {
            RegisterUsedNumber(state, state.OrderSlotNumber, definition.Number);
        }

        state.NumberDiscard.Add(state.OrderSlotNumber);
    }

    private void MoveSkillSlots(BattleState state)
    {
        for (int i = 0; i < state.SkillSlots.Count; i++)
        {
            SkillSlotState slot = state.SkillSlots[i];

            for (int numberIndex = 0; numberIndex < slot.NumberCards.Count; numberIndex++)
            {
                CardInstance numberCard = slot.NumberCards[numberIndex];
                NumCardDefinition definition = BattleState.GetNumDefinition(numberCard);

                if (definition != null)
                {
                    RegisterUsedNumber(state, numberCard, definition.Number);
                }

                state.NumberDiscard.Add(numberCard);
            }

            state.SkillDiscard.Add(slot.SkillCard);
        }
    }

    private void RegisterUsedNumber(BattleState state, CardInstance numberCard, int number)
    {
        if (state.UsedNumberCardsThisTurn.Contains(numberCard))
        {
            return;
        }

        state.UsedNumberCardsThisTurn.Add(numberCard);
        state.UsedNumbersThisTurn.Add(number);
    }
}
