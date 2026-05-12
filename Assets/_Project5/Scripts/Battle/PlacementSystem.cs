public class PlacementSystem
{
    public bool PlaceNumberInOrderSlot(BattleState state, CardInstance numberCard)
    {
        if (CanUseNumberFromHand(state, numberCard) == false)
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

    public bool PlaceSkillCard(BattleState state, CardInstance skillCard)
    {
        if (skillCard == null || skillCard.HasTag(CardTag.Skill) == false)
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
        if (CanUseNumberFromHand(state, numberCard) == false)
        {
            return false;
        }

        SkillSlotState slot = state.GetSkillSlot(skillCard);
        if (slot == null)
        {
            return false;
        }

        SkillCardDefinition skillDefinition = skillCard.Definition as SkillCardDefinition;
        if (skillDefinition == null)
        {
            return false;
        }

        if (slot.NumberCards.Count >= skillDefinition.RequiredNumberCount)
        {
            return false;
        }

        state.NumberHand.Remove(numberCard);
        slot.AddNumber(numberCard);
        return true;
    }

    public bool ReturnNumberFromOrderSlot(BattleState state)
    {
        if (state.OrderSlotNumber == null)
        {
            return false;
        }

        state.NumberHand.Add(state.OrderSlotNumber);
        state.OrderSlotNumber = null;
        return true;
    }

    private bool CanUseNumberFromHand(BattleState state, CardInstance numberCard)
    {
        if (numberCard == null || numberCard.HasTag(CardTag.Number) == false)
        {
            return false;
        }

        return state.NumberHand.Cards.Contains(numberCard);
    }
}
