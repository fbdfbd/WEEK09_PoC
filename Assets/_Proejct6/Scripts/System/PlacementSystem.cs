public class PlacementSystem
{
    public bool PlaceNumberInOrderSlot(BattleState state, CardInstance numberCard)
    {
        if (IsNumberInHand(state, numberCard) == false)
        {
            return false;
        }

        if (state.OrderSlotNumber != null)
        {
            state.NumberHand.Add(state.OrderSlotNumber);
        }

        state.NumberHand.Remove(numberCard);
        state.OrderSlotNumber = numberCard;
        return true;
    }

    public bool ReturnOrderSlotNumber(BattleState state)
    {
        if (state.OrderSlotNumber == null)
        {
            return false;
        }

        state.NumberHand.Add(state.OrderSlotNumber);
        state.OrderSlotNumber = null;
        return true;
    }

    public bool PlaceSkillCard(BattleState state, CardInstance skillCard)
    {
        SkillCardDefinition definition = BattleState.GetSkillDefinition(skillCard);
        if (definition == null)
        {
            return false;
        }

        if (state.SkillHand.Remove(skillCard) == false)
        {
            return false;
        }

        state.SkillSlots.Add(new SkillSlotState(skillCard));
        return true;
    }

    public bool PlaceNumberOnSkill(BattleState state, CardInstance skillCard, CardInstance numberCard)
    {
        if (IsNumberInHand(state, numberCard) == false)
        {
            return false;
        }

        SkillSlotState slot = state.GetSkillSlot(skillCard);
        if (slot == null)
        {
            return false;
        }

        SkillCardDefinition skillDefinition = BattleState.GetSkillDefinition(skillCard);
        if (skillDefinition == null)
        {
            return false;
        }

        if (slot.NumberCards.Count >= skillDefinition.RequiredCount)
        {
            return false;
        }

        state.NumberHand.Remove(numberCard);
        slot.AddNumber(numberCard);
        return true;
    }

    public bool ReturnNumberFromSkill(BattleState state, CardInstance skillCard, CardInstance numberCard)
    {
        SkillSlotState slot = state.GetSkillSlot(skillCard);
        if (slot == null)
        {
            return false;
        }

        if (slot.RemoveNumber(numberCard) == false)
        {
            return false;
        }

        state.NumberHand.Add(numberCard);
        return true;
    }

    private bool IsNumberInHand(BattleState state, CardInstance numberCard)
    {
        if (BattleState.GetNumDefinition(numberCard) == null)
        {
            return false;
        }

        return state.NumberHand.Contains(numberCard);
    }
}
