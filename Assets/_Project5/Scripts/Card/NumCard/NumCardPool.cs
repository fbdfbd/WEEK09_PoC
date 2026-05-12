using UnityEngine;

[CreateAssetMenu(fileName = "NumCardPool", menuName = "Project5/Cards/Num Card Pool")]
public class NumCardPool : ScriptableObject
{
    [SerializeField] private NumCardDefinition[] cards;

    public NumCardDefinition[] Cards
    {
        get { return cards; }
    }

    public NumCardDefinition GetCard(int value)
    {
        if (cards == null)
        {
            return null;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null && cards[i].Value == value)
            {
                return cards[i];
            }
        }

        return null;
    }

    public void Setup(NumCardDefinition[] newCards)
    {
        cards = newCards;
    }
}
