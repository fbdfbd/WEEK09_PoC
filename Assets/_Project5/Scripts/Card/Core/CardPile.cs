using System.Collections.Generic;
using UnityEngine;

public class CardPile
{
    private readonly List<CardInstance> cards = new List<CardInstance>();

    public int Count
    {
        get { return cards.Count; }
    }

    public List<CardInstance> Cards
    {
        get { return cards; }
    }

    public void Add(CardInstance card)
    {
        if (card == null)
        {
            return;
        }

        cards.Add(card);
    }

    public void AddRange(List<CardInstance> newCards)
    {
        if (newCards == null)
        {
            return;
        }

        for (int i = 0; i < newCards.Count; i++)
        {
            Add(newCards[i]);
        }
    }

    public bool Remove(CardInstance card)
    {
        return cards.Remove(card);
    }

    public CardInstance DrawTop()
    {
        if (cards.Count == 0)
        {
            return null;
        }

        CardInstance card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public void Clear()
    {
        cards.Clear();
    }

    public List<CardInstance> GetCards()
    {
        return new List<CardInstance>(cards);
    }

    public void SortByDisplayName()
    {
        cards.Sort(CompareByDisplayName);
    }

    public void SortNumberCards()
    {
        cards.Sort(CompareByNumberValue);
    }

    public void Shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cards.Count);
            CardInstance temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    private int CompareByDisplayName(CardInstance left, CardInstance right)
    {
        string leftName = GetDisplayName(left);
        string rightName = GetDisplayName(right);
        return string.Compare(leftName, rightName, System.StringComparison.Ordinal);
    }

    private int CompareByNumberValue(CardInstance left, CardInstance right)
    {
        int leftValue = GetNumberValue(left);
        int rightValue = GetNumberValue(right);
        return leftValue.CompareTo(rightValue);
    }

    private string GetDisplayName(CardInstance card)
    {
        if (card == null || card.Definition == null)
        {
            return string.Empty;
        }

        return card.Definition.DisplayName;
    }

    private int GetNumberValue(CardInstance card)
    {
        NumCardDefinition definition = null;
        if (card != null)
        {
            definition = card.Definition as NumCardDefinition;
        }

        if (definition == null)
        {
            return 0;
        }

        return definition.Value;
    }
}
