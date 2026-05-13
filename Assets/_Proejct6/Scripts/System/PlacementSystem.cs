public class PlacementSystem
{
    private readonly NumberRequirementChecker requirementChecker = new NumberRequirementChecker();

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
        return EnsureSkillSlot(state, skillCard) != null;
    }

    public bool PlaceNumberOnSkill(BattleState state, CardInstance skillCard, CardInstance numberCard)
    {
        if (IsNumberInHand(state, numberCard) == false)
        {
            return false;
        }

        SkillSlotState slot = state.GetSkillSlot(skillCard);
        SkillCardDefinition skillDefinition = BattleState.GetSkillDefinition(skillCard);
        if (skillDefinition == null)
        {
            return false;
        }

        if (requirementChecker.CanUseNumber(skillDefinition, numberCard) == false)
        {
            return false;
        }

        slot = EnsureSkillSlot(state, skillCard);
        if (slot == null)
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

    public bool PlaceOrSwapNumberOnSkill(BattleState state, CardInstance skillCard, CardInstance numberCard)
    {
        if (IsNumberInHand(state, numberCard) == false)
        {
            return false;
        }

        SkillSlotState slot = state.GetSkillSlot(skillCard);
        SkillCardDefinition skillDefinition = BattleState.GetSkillDefinition(skillCard);
        if (skillDefinition == null || skillDefinition.RequiredCount <= 0)
        {
            return false;
        }

        if (requirementChecker.CanUseNumber(skillDefinition, numberCard) == false)
        {
            return false;
        }

        slot = EnsureSkillSlot(state, skillCard);
        if (slot == null)
        {
            return false;
        }

        state.NumberHand.Remove(numberCard);

        if (slot.NumberCards.Count < skillDefinition.RequiredCount)
        {
            slot.AddNumber(numberCard);
            return true;
        }

        int replaceIndex = slot.NumberCards.Count - 1;
        CardInstance replacedCard = slot.ReplaceNumberAt(replaceIndex, numberCard);
        state.NumberHand.Add(replacedCard);
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

    public bool ReturnAllNumbersFromSkill(BattleState state, CardInstance skillCard)
    {
        SkillSlotState slot = state.GetSkillSlot(skillCard);
        if (slot == null)
        {
            return false;
        }

        while (slot.NumberCards.Count > 0)
        {
            CardInstance numberCard = slot.NumberCards[slot.NumberCards.Count - 1];
            slot.RemoveNumber(numberCard);
            state.NumberHand.Add(numberCard);
        }

        state.SkillSlots.Remove(slot);
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

    private SkillSlotState EnsureSkillSlot(BattleState state, CardInstance skillCard)
    {
        if (BattleState.GetSkillDefinition(skillCard) == null)
        {
            return null;
        }

        if (state.SkillHand.Contains(skillCard) == false)
        {
            return null;
        }

        SkillSlotState slot = state.GetSkillSlot(skillCard);
        if (slot != null)
        {
            return slot;
        }

        slot = new SkillSlotState(skillCard);
        state.SkillSlots.Add(slot);
        return slot;
    }
}
