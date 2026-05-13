using System.Collections.Generic;

public class SkillResolveData
{
    public CardInstance SkillCard { get; private set; }
    public List<CardInstance> NumberCards { get; private set; }
    public List<int> Numbers { get; private set; } = new List<int>();
    public int NumberSum { get; private set; }

    public SkillResolveData(CardInstance skillCard, List<CardInstance> numberCards)
    {
        SkillCard = skillCard;
        NumberCards = numberCards;
        BuildNumbers();
    }

    private void BuildNumbers()
    {
        if (NumberCards == null)
        {
            return;
        }

        for (int i = 0; i < NumberCards.Count; i++)
        {
            NumCardDefinition definition = BattleState.GetNumDefinition(NumberCards[i]);
            if (definition == null)
            {
                continue;
            }

            Numbers.Add(definition.Number);
            NumberSum += definition.Number;
        }
    }
}
