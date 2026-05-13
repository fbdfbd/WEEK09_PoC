using System.Collections.Generic;

public class SkillSlotState
{
    public CardInstance SkillCard { get; private set; }
    public List<CardInstance> NumberCards { get; private set; } = new List<CardInstance>();

    public SkillSlotState(CardInstance skillCard)
    {
        SkillCard = skillCard;
    }

    public void AddNumber(CardInstance numberCard)
    {
        if (numberCard == null)
        {
            return;
        }

        NumberCards.Add(numberCard);
    }

    public bool RemoveNumber(CardInstance numberCard)
    {
        return NumberCards.Remove(numberCard);
    }

    public CardInstance ReplaceNumberAt(int index, CardInstance newNumberCard)
    {
        if (index < 0 || index >= NumberCards.Count)
        {
            return null;
        }

        CardInstance oldNumberCard = NumberCards[index];
        NumberCards[index] = newNumberCard;
        return oldNumberCard;
    }
}
