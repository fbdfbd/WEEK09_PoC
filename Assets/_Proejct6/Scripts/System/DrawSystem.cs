public class DrawSystem
{
    private const int MaxNumberHandCount = 10;

    public void DrawNumbers(BattleState state)
    {
        int drawCount = state.GetNextNumberDrawCount();
        DrawNumberCards(state, drawCount);
    }

    public void DrawNumber(BattleState state)
    {
        DrawNumberCards(state, 1);
    }

    public void DrawSkills(BattleState state, int drawCount)
    {
        DrawCards(state.SkillDeck, state.SkillHand, state.SkillDiscard, drawCount);
    }

    public void DrawSkill(BattleState state)
    {
        DrawCards(state.SkillDeck, state.SkillHand, state.SkillDiscard, 1);
    }

    private void DrawNumberCards(BattleState state, int drawCount)
    {
        int availableSpace = MaxNumberHandCount - state.NumberHand.Count;
        if (availableSpace <= 0)
        {
            return;
        }

        if (drawCount > availableSpace)
        {
            drawCount = availableSpace;
        }

        DrawCards(state.NumberDeck, state.NumberHand, state.NumberDiscard, drawCount);
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
            CardInstance card = discard.DrawTop();
            deck.Add(card);
        }

        deck.Shuffle();
    }
}
