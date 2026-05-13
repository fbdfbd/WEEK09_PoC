public class NumberDeckBuilder
{
    public void BuildDefaultNumberDeck(BattleState state, NumCardPool numCardPool)
    {
        if (state == null || numCardPool == null)
        {
            return;
        }

        state.NumberDeck.Clear();

        for (int number = 1; number <= 10; number++)
        {
            NumCardDefinition definition = numCardPool.GetByNumber(number);
            AddTwoCopies(state, definition);
        }

        state.NumberDeck.Shuffle();
    }

    private void AddTwoCopies(BattleState state, NumCardDefinition definition)
    {
        if (definition == null)
        {
            return;
        }

        state.NumberDeck.Add(new CardInstance(definition));
        state.NumberDeck.Add(new CardInstance(definition));
    }
}
