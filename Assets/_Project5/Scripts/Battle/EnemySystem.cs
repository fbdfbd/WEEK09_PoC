public class EnemySystem
{
    public EnemyIntent CreateIntent(BattleState state)
    {
        EnemyIntent intent = new EnemyIntent();
        intent.Type = EnemyIntentType.Attack;
        intent.SkillCard = ChooseFirstSkillCard(state);
        intent.OrderCard = ChooseHighestNumberCard(state);
        intent.OrderValue = GetNumberValue(intent.OrderCard);
        intent.Power = GetSkillPower(intent.SkillCard, intent.OrderCard);
        return intent;
    }

    public void ApplyIntent(BattleContext context, EnemyIntent intent)
    {
        if (intent == null)
        {
            return;
        }

        if (intent.Type == EnemyIntentType.Attack)
        {
            context.Player.ActivateReservedBlock();
            context.Player.TakeDamage(intent.Power);
        }

        if (intent.Type == EnemyIntentType.Defend)
        {
            context.Enemy.AddBlock(intent.Power);
        }
    }

    public void PrepareEnemyCards(BattleState state)
    {
        CardInstance orderCard = ChooseHighestNumberCard(state);
        if (orderCard != null)
        {
            state.EnemyNumberHand.Remove(orderCard);
            state.EnemyOrderSlotNumber = orderCard;
            state.EnemyUsedNumbersThisTurn.Add(GetNumberValue(orderCard));
            state.EnemyUsedNumberCardsThisTurn.Add(orderCard);
        }

        CardInstance skillCard = ChooseFirstSkillCard(state);
        CardInstance numberCard = ChooseHighestNumberCard(state);

        if (skillCard == null || numberCard == null)
        {
            return;
        }

        state.EnemySkillHand.Remove(skillCard);
        state.EnemyNumberHand.Remove(numberCard);

        SkillSlotState slot = new SkillSlotState(skillCard);
        slot.AddNumber(numberCard);
        state.EnemySkillSlots.Add(slot);

        state.EnemyUsedNumbersThisTurn.Add(GetNumberValue(numberCard));
        state.EnemyUsedNumberCardsThisTurn.Add(numberCard);
        state.EnemyUsedSkillCardsThisTurn.Add(skillCard);
    }

    public void MoveEnemyUsedCardsToDiscard(BattleState state)
    {
        if (state.EnemyOrderSlotNumber != null)
        {
            state.EnemyNumberDiscard.Add(state.EnemyOrderSlotNumber);
            state.EnemyOrderSlotNumber = null;
        }

        for (int i = 0; i < state.EnemySkillSlots.Count; i++)
        {
            SkillSlotState slot = state.EnemySkillSlots[i];

            for (int numberIndex = 0; numberIndex < slot.NumberCards.Count; numberIndex++)
            {
                state.EnemyNumberDiscard.Add(slot.NumberCards[numberIndex]);
            }

            state.EnemySkillDiscard.Add(slot.SkillCard);
        }
    }

    private CardInstance ChooseFirstSkillCard(BattleState state)
    {
        if (state.EnemySkillHand.Count == 0)
        {
            return null;
        }

        return state.EnemySkillHand.Cards[0];
    }

    private CardInstance ChooseHighestNumberCard(BattleState state)
    {
        CardInstance bestCard = null;
        int bestValue = -1;

        for (int i = 0; i < state.EnemyNumberHand.Cards.Count; i++)
        {
            CardInstance card = state.EnemyNumberHand.Cards[i];
            int value = GetNumberValue(card);

            if (value > bestValue)
            {
                bestValue = value;
                bestCard = card;
            }
        }

        return bestCard;
    }

    private int GetNumberValue(CardInstance card)
    {
        NumCardDefinition definition = BattleState.GetNumDefinition(card);
        if (definition == null)
        {
            return 0;
        }

        return definition.Value;
    }

    private int GetSkillPower(CardInstance skillCard, CardInstance numberCard)
    {
        SkillCardDefinition skillDefinition = null;
        if (skillCard != null)
        {
            skillDefinition = skillCard.Definition as SkillCardDefinition;
        }

        if (skillDefinition == null || skillDefinition.Role != SkillCardRole.Attack)
        {
            return 0;
        }

        return GetNumberValue(numberCard);
    }
}
