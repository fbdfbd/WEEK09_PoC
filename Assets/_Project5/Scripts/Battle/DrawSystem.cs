public class DrawSystem
{
    public void DrawNumbers(BattleState state, int drawCount, int maxHandCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            if (state.NumberHand.Count >= maxHandCount)
            {
                return;
            }

            RefillDeckIfNeeded(state.NumberDeck, state.NumberDiscard);
            CardInstance card = state.NumberDeck.DrawTop();
            state.NumberHand.Add(card);
        }
    }

    public void DrawSkills(BattleState state, int drawCount, int maxHandCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            if (state.SkillHand.Count >= maxHandCount)
            {
                return;
            }

            RefillDeckIfNeeded(state.SkillDeck, state.SkillDiscard);
            CardInstance card = state.SkillDeck.DrawTop();
            state.SkillHand.Add(card);
        }
    }

    public void DrawNumbersForEnemy(BattleState state, int drawCount, int maxHandCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            if (state.EnemyNumberHand.Count >= maxHandCount)
            {
                return;
            }

            RefillDeckIfNeeded(state.EnemyNumberDeck, state.EnemyNumberDiscard);
            CardInstance card = state.EnemyNumberDeck.DrawTop();
            state.EnemyNumberHand.Add(card);
        }
    }

    public void DrawSkillsForEnemy(BattleState state, int drawCount, int maxHandCount)
    {
        for (int i = 0; i < drawCount; i++)
        {
            if (state.EnemySkillHand.Count >= maxHandCount)
            {
                return;
            }

            RefillDeckIfNeeded(state.EnemySkillDeck, state.EnemySkillDiscard);
            CardInstance card = state.EnemySkillDeck.DrawTop();
            state.EnemySkillHand.Add(card);
        }
    }

    private void RefillDeckIfNeeded(CardPile deck, CardPile discard)
    {
        if (deck.Count > 0)
        {
            return;
        }

        deck.AddRange(discard.GetCards());
        discard.Clear();
        deck.Shuffle();
    }
}
