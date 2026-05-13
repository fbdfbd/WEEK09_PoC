using System.Collections.Generic;
using UnityEngine;

public class CardPile
{
    /// <summary>
    /// 런타임중 가지고 있는 카드 리스트
    /// </summary>
    private readonly List<CardInstance> cards = new List<CardInstance>();


    public int Count
    {
        get { return cards.Count; }
    }

    public void Add(CardInstance card)
    {
        if (card == null)
        {
            return;
        }

        cards.Add(card);
    }

    public bool Remove(CardInstance card)
    {
        return cards.Remove(card);
    }

    public bool Contains(CardInstance card)
    {
        return cards.Contains(card);
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

    public CardInstance DrawBottom()
    {
        if (cards.Count == 0)
        {
            return null;
        }

        int lastIndex = cards.Count - 1;
        CardInstance card = cards[lastIndex];
        cards.RemoveAt(lastIndex);
        return card;
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

    public void Clear()
    {
        cards.Clear();
    }

    public IReadOnlyList<CardInstance> GetCards()
    {
        return cards.AsReadOnly();
    }
}
