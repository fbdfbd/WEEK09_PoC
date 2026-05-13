using System.Collections.Generic;

public class EnemyCardSystem
{
    private readonly NumberRequirementChecker requirementChecker = new NumberRequirementChecker();

    public void BuildDecks(BattleState state, NumCardPool numCardPool, IReadOnlyList<SkillCardDefinition> skillDefinitions)
    {
        BuildNumberDeck(state, numCardPool);
        BuildSkillDeck(state, skillDefinitions);
    }

    public void StartTurn(BattleState state)
    {
        DrawEnemyNumbers(state, state.GetNextEnemyNumberDrawCount());
        DrawEnemySkills(state, 3);
        PrepareEnemyTurn(state);
    }

    public void ResolveEnemySkills(BattleState state)
    {
        for (int i = 0; i < state.EnemySkillSlots.Count; i++)
        {
            ResolveEnemySkillSlot(state, state.EnemySkillSlots[i]);
        }
    }

    public void EndTurn(BattleState state)
    {
        MoveEnemyOrderNumber(state);
        MoveEnemySkillSlots(state);

        state.EnemyLastTurnUsedNumberCount = state.EnemyUsedNumbersThisTurn.Count;
        state.EnemyUsedNumbersThisTurn.Clear();
        state.EnemyUsedNumberCardsThisTurn.Clear();
        state.EnemyUsedSkillCardsThisTurn.Clear();
        state.EnemySkillSlots.Clear();
        state.EnemyOrderSlotNumber = null;
    }

    private void BuildNumberDeck(BattleState state, NumCardPool numCardPool)
    {
        state.EnemyNumberDeck.Clear();

        if (numCardPool == null)
        {
            return;
        }

        for (int number = 1; number <= 10; number++)
        {
            NumCardDefinition definition = numCardPool.GetByNumber(number);
            if (definition == null)
            {
                continue;
            }

            state.EnemyNumberDeck.Add(new CardInstance(definition));
            state.EnemyNumberDeck.Add(new CardInstance(definition));
        }

        state.EnemyNumberDeck.Shuffle();
    }

    private void BuildSkillDeck(BattleState state, IReadOnlyList<SkillCardDefinition> skillDefinitions)
    {
        state.EnemySkillDeck.Clear();

        if (skillDefinitions == null)
        {
            return;
        }

        for (int i = 0; i < skillDefinitions.Count; i++)
        {
            if (skillDefinitions[i] != null)
            {
                state.EnemySkillDeck.Add(new CardInstance(skillDefinitions[i]));
            }
        }

        state.EnemySkillDeck.Shuffle();
    }

    private void DrawEnemyNumbers(BattleState state, int drawCount)
    {
        DrawCards(state.EnemyNumberDeck, state.EnemyNumberHand, state.EnemyNumberDiscard, drawCount);
    }

    private void DrawEnemySkills(BattleState state, int drawCount)
    {
        DrawCards(state.EnemySkillDeck, state.EnemySkillHand, state.EnemySkillDiscard, drawCount);
    }

    private void DrawCards(CardPile deck, CardPile hand, CardPile discard, int drawCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            RefillDeckIfNeeded(deck, discard);

            CardInstance card = deck.DrawTop();
            if (card == null)
            {
                return;
            }

            hand.Add(card);
        }
    }

    private void RefillDeckIfNeeded(CardPile deck, CardPile discard)
    {
        if (deck.Count > 0)
        {
            return;
        }

        while (discard.Count > 0)
        {
            deck.Add(discard.DrawTop());
        }

        deck.Shuffle();
    }

    private void PrepareEnemyTurn(BattleState state)
    {
        PlaceEnemyOrderNumber(state);
        PlaceEnemySkillNumbers(state);
    }

    private void PlaceEnemyOrderNumber(BattleState state)
    {
        CardInstance numberCard = FindHighestNumber(state.EnemyNumberHand);
        if (numberCard == null)
        {
            return;
        }

        state.EnemyNumberHand.Remove(numberCard);
        state.EnemyOrderSlotNumber = numberCard;
    }

    private void PlaceEnemySkillNumbers(BattleState state)
    {
        IReadOnlyList<CardInstance> skillCards = state.EnemySkillHand.GetCards();

        for (int skillIndex = 0; skillIndex < skillCards.Count; skillIndex++)
        {
            CardInstance skillCard = skillCards[skillIndex];
            SkillCardDefinition skill = BattleState.GetSkillDefinition(skillCard);
            if (skill == null)
            {
                continue;
            }

            SkillSlotState slot = BuildEnemySkillSlot(state, skillCard, skill);
            if (slot == null)
            {
                continue;
            }

            state.EnemySkillSlots.Add(slot);
        }
    }

    private SkillSlotState BuildEnemySkillSlot(BattleState state, CardInstance skillCard, SkillCardDefinition skill)
    {
        SkillSlotState slot = new SkillSlotState(skillCard);

        for (int i = 0; i < skill.RequiredCount; i++)
        {
            CardInstance numberCard = FindHighestUsableNumber(state.EnemyNumberHand, skill);
            if (numberCard == null)
            {
                ReturnSlotNumbersToHand(state, slot);
                return null;
            }

            state.EnemyNumberHand.Remove(numberCard);
            slot.AddNumber(numberCard);
        }

        return slot;
    }

    private void ReturnSlotNumbersToHand(BattleState state, SkillSlotState slot)
    {
        for (int i = 0; i < slot.NumberCards.Count; i++)
        {
            state.EnemyNumberHand.Add(slot.NumberCards[i]);
        }
    }

    private CardInstance FindHighestNumber(CardPile pile)
    {
        CardInstance bestCard = null;
        int bestNumber = -1;
        IReadOnlyList<CardInstance> cards = pile.GetCards();

        for (int i = 0; i < cards.Count; i++)
        {
            NumCardDefinition number = BattleState.GetNumDefinition(cards[i]);
            if (number != null && number.Number > bestNumber)
            {
                bestNumber = number.Number;
                bestCard = cards[i];
            }
        }

        return bestCard;
    }

    private CardInstance FindHighestUsableNumber(CardPile pile, SkillCardDefinition skill)
    {
        CardInstance bestCard = null;
        int bestNumber = -1;
        IReadOnlyList<CardInstance> cards = pile.GetCards();

        for (int i = 0; i < cards.Count; i++)
        {
            NumCardDefinition number = BattleState.GetNumDefinition(cards[i]);
            if (number == null)
            {
                continue;
            }

            if (requirementChecker.CanUseNumber(skill, cards[i]) && number.Number > bestNumber)
            {
                bestNumber = number.Number;
                bestCard = cards[i];
            }
        }

        return bestCard;
    }

    private void ResolveEnemySkillSlot(BattleState state, SkillSlotState slot)
    {
        SkillCardDefinition skill = BattleState.GetSkillDefinition(slot.SkillCard);
        if (skill == null)
        {
            return;
        }

        if (slot.NumberCards.Count < skill.RequiredCount)
        {
            return;
        }

        int numberSum = GetNumberSum(slot);
        state.Log.Add("적이 " + skill.DisplayName + "에 " + BuildNumberListText(slot) + "을 장착했습니다.");

        if (skill.Role == SkillCardRole.Attack || skill.Role == SkillCardRole.HeavyAttack || skill.Role == SkillCardRole.AreaAttack)
        {
            state.Player.TakeDamage(numberSum);
            state.Log.Add("적이 " + skill.DisplayName + "로 플레이어에게 " + numberSum + " 피해를 줬습니다.");
        }
        else if (skill.Role == SkillCardRole.Defense || skill.Role == SkillCardRole.Counter)
        {
            state.Enemy.AddBlock(numberSum);
            state.Log.Add("적이 " + skill.DisplayName + "로 방어도 " + numberSum + "을 얻었습니다.");
        }
        else if (skill.Role == SkillCardRole.Heal)
        {
            state.Enemy.Heal(numberSum);
            state.Log.Add("적이 " + skill.DisplayName + "로 체력 " + numberSum + "을 회복했습니다.");
        }

        RegisterEnemyUsedCards(state, slot);
    }

    private string BuildNumberListText(SkillSlotState slot)
    {
        string text = string.Empty;

        for (int i = 0; i < slot.NumberCards.Count; i++)
        {
            if (text.Length > 0)
            {
                text += ", ";
            }

            text += GetNumberValue(slot.NumberCards[i]).ToString();
        }

        return "숫자 " + text;
    }

    private int GetNumberSum(SkillSlotState slot)
    {
        int sum = 0;

        for (int i = 0; i < slot.NumberCards.Count; i++)
        {
            NumCardDefinition number = BattleState.GetNumDefinition(slot.NumberCards[i]);
            if (number != null)
            {
                sum += number.Number;
            }
        }

        return sum;
    }

    private int GetNumberValue(CardInstance numberCard)
    {
        NumCardDefinition number = BattleState.GetNumDefinition(numberCard);
        if (number == null)
        {
            return 0;
        }

        return number.Number;
    }

    private void RegisterEnemyUsedCards(BattleState state, SkillSlotState slot)
    {
        if (state.EnemyUsedSkillCardsThisTurn.Contains(slot.SkillCard) == false)
        {
            state.EnemyUsedSkillCardsThisTurn.Add(slot.SkillCard);
        }

        for (int i = 0; i < slot.NumberCards.Count; i++)
        {
            CardInstance numberCard = slot.NumberCards[i];
            NumCardDefinition number = BattleState.GetNumDefinition(numberCard);

            if (numberCard != null && state.EnemyUsedNumberCardsThisTurn.Contains(numberCard) == false)
            {
                state.EnemyUsedNumberCardsThisTurn.Add(numberCard);
            }

            if (number != null)
            {
                state.EnemyUsedNumbersThisTurn.Add(number.Number);
            }
        }
    }

    private void MoveEnemyOrderNumber(BattleState state)
    {
        if (state.EnemyOrderSlotNumber == null)
        {
            return;
        }

        RegisterEnemyOrderNumber(state);
        state.EnemyNumberDiscard.Add(state.EnemyOrderSlotNumber);
    }

    private void RegisterEnemyOrderNumber(BattleState state)
    {
        if (state.EnemyUsedNumberCardsThisTurn.Contains(state.EnemyOrderSlotNumber))
        {
            return;
        }

        NumCardDefinition number = BattleState.GetNumDefinition(state.EnemyOrderSlotNumber);
        if (number == null)
        {
            return;
        }

        state.EnemyUsedNumberCardsThisTurn.Add(state.EnemyOrderSlotNumber);
        state.EnemyUsedNumbersThisTurn.Add(number.Number);
    }

    private void MoveEnemySkillSlots(BattleState state)
    {
        for (int i = 0; i < state.EnemySkillSlots.Count; i++)
        {
            SkillSlotState slot = state.EnemySkillSlots[i];
            bool wasUsed = state.EnemyUsedSkillCardsThisTurn.Contains(slot.SkillCard);

            for (int numberIndex = 0; numberIndex < slot.NumberCards.Count; numberIndex++)
            {
                if (wasUsed)
                {
                    state.EnemyNumberDiscard.Add(slot.NumberCards[numberIndex]);
                }
                else
                {
                    state.EnemyNumberHand.Add(slot.NumberCards[numberIndex]);
                }
            }

            if (wasUsed)
            {
                state.EnemySkillHand.Remove(slot.SkillCard);
                state.EnemySkillDiscard.Add(slot.SkillCard);
            }
        }
    }
}
