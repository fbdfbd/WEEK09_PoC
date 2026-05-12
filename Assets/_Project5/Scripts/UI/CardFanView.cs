using System;
using System.Collections.Generic;
using UnityEngine;

public class CardFanView : MonoBehaviour
{
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private RectTransform cardRoot;
    [SerializeField] private float radius = 500f;
    [SerializeField] private float maxFanAngle = 55f;
    [SerializeField] private float maxAngleStep = 12f;
    [SerializeField] private float verticalOffset;
    [SerializeField] private float cardScale = 1f;

    private readonly List<CardView> cardViews = new List<CardView>();
    private readonly List<CardInstance> currentCards = new List<CardInstance>();

    public event Action<CardInstance> CardClicked;

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
        SortSiblingOrder();
        RefreshLayout();
    }

    public void AddCard(CardInstance card)
    {
        if (card == null)
        {
            return;
        }

        currentCards.Add(card);
        ShowCards(currentCards);
    }

    public void RemoveCard(CardInstance card)
    {
        if (card == null)
        {
            return;
        }

        currentCards.Remove(card);
        ShowCards(currentCards);
    }

    public void Clear()
    {
        currentCards.Clear();
        ShowCards(currentCards);
    }

    public void RefreshLayout()
    {
        int count = cardViews.Count;
        if (count == 0)
        {
            return;
        }

        float angleStep = GetAngleStep(count);
        float startAngle = -angleStep * (count - 1) * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + angleStep * i;
            ApplyFanTransform(cardViews[i].transform as RectTransform, angle);
        }
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
            Debug.LogError("CardFanView needs a card prefab.");
            return;
        }

        RectTransform parent = cardRoot;
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

    private void SortSiblingOrder()
    {
        for (int i = 0; i < cardViews.Count; i++)
        {
            cardViews[i].transform.SetSiblingIndex(i);
        }
    }

    private float GetAngleStep(int count)
    {
        if (count <= 1)
        {
            return 0f;
        }

        float angleStep = maxFanAngle / (count - 1);
        if (angleStep > maxAngleStep)
        {
            angleStep = maxAngleStep;
        }

        return angleStep;
    }

    private void ApplyFanTransform(RectTransform cardTransform, float angle)
    {
        if (cardTransform == null)
        {
            return;
        }

        float radians = angle * Mathf.Deg2Rad;
        float x = Mathf.Sin(radians) * radius;
        float y = Mathf.Cos(radians) * radius - radius + verticalOffset;

        cardTransform.anchoredPosition = new Vector2(x, y);
        cardTransform.localRotation = Quaternion.Euler(0f, 0f, -angle);
        cardTransform.localScale = new Vector3(cardScale, cardScale, 1f);
    }

    private void OnCardViewClicked(CardView view)
    {
        if (view == null)
        {
            return;
        }

        if (CardClicked != null)
        {
            CardClicked(view.Card);
        }
    }
}
