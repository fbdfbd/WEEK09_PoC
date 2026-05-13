using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Num Card Pool")]
public class NumCardPool : ScriptableObject
{
    [SerializeField] private List<NumCardDefinition> _cards;

    public List<NumCardDefinition> Cards => _cards;

    public NumCardDefinition GetByNumber(int number)
    {
        foreach (NumCardDefinition card in _cards)
        {
            if (card.Number == number)
            {
                return card;
            }
        }

        return null;
    }
}
