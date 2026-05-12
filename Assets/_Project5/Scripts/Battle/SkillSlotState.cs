using System.Collections.Generic;

public class SkillSlotState
{
    private readonly CardInstance skillCard;
    private readonly List<CardInstance> numberCards = new List<CardInstance>();

    public CardInstance SkillCard
    {
        get { return skillCard; }
    }

    public List<CardInstance> NumberCards
    {
        get { return numberCards; }
    }

    public SkillSlotState(CardInstance skillCard)
    {
        this.skillCard = skillCard;
    }

    public void AddNumber(CardInstance numberCard)
    {
        if (numberCard == null)
        {
            return;
        }

        numberCards.Add(numberCard);
    }

    public void ClearNumbers()
    {
        numberCards.Clear();
    }
}
