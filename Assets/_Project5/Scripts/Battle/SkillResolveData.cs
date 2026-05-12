using System.Collections.Generic;

public class SkillResolveData
{
    private readonly CardInstance skillCard;
    private readonly List<CardInstance> numberCards;
    private readonly List<int> numbers = new List<int>();
    private int numberSum;

    public CardInstance SkillCard
    {
        get { return skillCard; }
    }

    public List<CardInstance> NumberCards
    {
        get { return numberCards; }
    }

    public List<int> Numbers
    {
        get { return numbers; }
    }

    public int NumberSum
    {
        get { return numberSum; }
    }

    public SkillResolveData(CardInstance skillCard, List<CardInstance> numberCards)
    {
        this.skillCard = skillCard;
        this.numberCards = numberCards;
        BuildNumberValues();
    }

    private void BuildNumberValues()
    {
        if (numberCards == null)
        {
            return;
        }

        for (int i = 0; i < numberCards.Count; i++)
        {
            NumCardDefinition definition = BattleState.GetNumDefinition(numberCards[i]);
            if (definition != null)
            {
                numbers.Add(definition.Value);
                numberSum += definition.Value;
            }
        }
    }
}
