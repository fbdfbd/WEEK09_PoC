using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SkillHandScrollView : MonoBehaviour
{
    [SerializeField] private SkillCardView _cardPrefab;
    [SerializeField] private RectTransform _contentRoot;
    [SerializeField] private float _sortTweenDuration = 0.15f;

    private readonly List<SkillCardView> _views = new List<SkillCardView>();
    private readonly List<CardInstance> _cards = new List<CardInstance>();

    public event Action<CardInstance> SkillCardClicked;

    private IReadOnlyList<SkillSlotState> _currentSlots;

    public void ShowCards(IReadOnlyList<CardInstance> cards)
    {
        ShowCards(cards, null);
    }

    public void ShowCards(IReadOnlyList<CardInstance> cards, IReadOnlyList<SkillSlotState> slots)
    {
        _currentSlots = slots;
        _cards.Clear();

        if (cards != null)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                _cards.Add(cards[i]);
            }
        }

        SyncViewCount(_cards.Count);
        BindCards();
        RefreshViews();
    }

    private void SyncViewCount(int targetCount)
    {
        while (_views.Count < targetCount)
        {
            CreateView();
        }
    }

    private void CreateView()
    {
        if (_cardPrefab == null)
        {
            Debug.LogError("SkillHandScrollView needs a card prefab.");
            return;
        }

        RectTransform parent = _contentRoot;
        if (parent == null)
        {
            parent = transform as RectTransform;
        }

        SkillCardView view = Instantiate(_cardPrefab, parent);
        view.Clicked += OnSkillCardClicked;
        _views.Add(view);
    }

    private void BindCards()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            _views[i].SetCard(_cards[i], FindSlot(_cards[i]));
        }
    }

    private SkillSlotState FindSlot(CardInstance skillCard)
    {
        if (_currentSlots == null)
        {
            return null;
        }

        for (int i = 0; i < _currentSlots.Count; i++)
        {
            if (_currentSlots[i].SkillCard == skillCard)
            {
                return _currentSlots[i];
            }
        }

        return null;
    }

    private void RefreshViews()
    {
        for (int i = 0; i < _views.Count; i++)
        {
            bool active = i < _cards.Count;
            _views[i].gameObject.SetActive(active);

            if (active)
            {
                _views[i].transform.SetSiblingIndex(i);
                _views[i].transform.DOKill();
                _views[i].transform.DOScale(Vector3.one, _sortTweenDuration).SetEase(Ease.OutCubic);
            }
        }
    }

    private void OnSkillCardClicked(CardInstance card)
    {
        if (SkillCardClicked != null)
        {
            SkillCardClicked(card);
        }
    }
}
