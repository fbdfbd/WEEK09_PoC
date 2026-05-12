using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillHandScrollView : MonoBehaviour
{
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private RectTransform contentRoot;

    private readonly List<CardView> cardViews = new List<CardView>();
    private readonly List<CardInstance> currentCards = new List<CardInstance>();

    public event Action<CardInstance> SkillCardClicked;

    public void ShowCards(List<CardInstance> cards)
    {
        currentCards.Clear();

        if (cards != null)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                currentCards.Add(cards[i]);
            }
        }

        SyncCardViewCount(currentCards.Count);
        BindCards();
        RefreshOrder();
    }

    public void Clear()
    {
        currentCards.Clear();
        ShowCards(currentCards);
    }

    public CardView GetCardView(CardInstance card)
    {
        for (int i = 0; i < cardViews.Count; i++)
        {
            if (cardViews[i].Card == card)
            {
                return cardViews[i];
            }
        }

        return null;
    }

    private void SyncCardViewCount(int targetCount)
    {
        while (cardViews.Count < targetCount)
        {
            CreateCardView();
        }

        for (int i = 0; i < cardViews.Count; i++)
        {
            bool shouldShow = i < targetCount;
            cardViews[i].gameObject.SetActive(shouldShow);
        }
    }

    private void CreateCardView()
    {
        if (cardPrefab == null)
        {
            Debug.LogError("SkillHandScrollView needs a card prefab.");
            return;
        }

        RectTransform parent = contentRoot;
        if (parent == null)
        {
            parent = transform as RectTransform;
        }

        CardView view = Instantiate(cardPrefab, parent);
        view.Clicked += OnCardViewClicked;
        cardViews.Add(view);
    }

    private void BindCards()
    {
        for (int i = 0; i < currentCards.Count; i++)
        {
            cardViews[i].SetCard(currentCards[i]);
        }
    }

    private void RefreshOrder()
    {
        for (int i = 0; i < cardViews.Count; i++)
        {
            cardViews[i].transform.SetSiblingIndex(i);
        }
    }

    private void OnCardViewClicked(CardView view)
    {
        if (view == null)
        {
            return;
        }

        if (SkillCardClicked != null)
        {
            SkillCardClicked(view.Card);
        }
    }
}
