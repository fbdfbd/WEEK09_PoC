public class SkillResolveSystem
{
    public void ReserveDefense(BattleContext context)
    {
        for (int i = 0; i < context.State.SkillSlots.Count; i++)
        {
            SkillSlotState slot = context.State.SkillSlots[i];
            SkillCardDefinition skill = slot.SkillCard.Definition as SkillCardDefinition;

            if (skill == null || skill.Role != SkillCardRole.Defense)
            {
                continue;
            }

            ApplySkillEffects(context, slot);
        }
    }

    public void ResolvePlayerSkills(BattleContext context)
    {
        for (int i = 0; i < context.State.SkillSlots.Count; i++)
        {
            SkillSlotState slot = context.State.SkillSlots[i];
            SkillCardDefinition skill = slot.SkillCard.Definition as SkillCardDefinition;

            if (skill == null)
            {
                continue;
            }

            if (skill.Role == SkillCardRole.Defense)
            {
                continue;
            }

            ApplySkillEffects(context, slot);
        }
    }

    private void ApplySkillEffects(BattleContext context, SkillSlotState slot)
    {
        SkillCardDefinition skill = slot.SkillCard.Definition as SkillCardDefinition;
        if (skill == null)
        {
            return;
        }

        if (slot.NumberCards.Count < skill.RequiredNumberCount)
        {
            return;
        }

        SkillResolveData data = new SkillResolveData(slot.SkillCard, slot.NumberCards);

        if (skill.Effects != null)
        {
            for (int i = 0; i < skill.Effects.Length; i++)
            {
                if (skill.Effects[i] != null)
                {
                    skill.Effects[i].Apply(context, data);
                }
            }
        }

        RegisterUsedCards(context.State, slot, data);
    }

    private void RegisterUsedCards(BattleState state, SkillSlotState slot, SkillResolveData data)
    {
        if (state.UsedSkillCardsThisTurn.Contains(slot.SkillCard) == false)
        {
            state.UsedSkillCardsThisTurn.Add(slot.SkillCard);
        }

        for (int i = 0; i < slot.NumberCards.Count; i++)
        {
            if (state.UsedNumberCardsThisTurn.Contains(slot.NumberCards[i]) == false)
            {
                state.UsedNumberCardsThisTurn.Add(slot.NumberCards[i]);
            }
        }

        for (int i = 0; i < data.Numbers.Count; i++)
        {
            state.UsedNumbersThisTurn.Add(data.Numbers[i]);
        }
    }
}
