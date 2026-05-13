using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumberCardHandView : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private NumberCardView _cardPrefab;
    [SerializeField] private RectTransform _cardRoot;
    [SerializeField] private RectTransform _dragRoot;
    [SerializeField] private float _radius = 520f;
    [SerializeField] private float _maxFanAngle = 60f;
    [SerializeField] private float _maxAngleStep = 12f;
    [SerializeField] private float _verticalOffset;
    [SerializeField] private float _cardScale = 1f;
    [SerializeField] private float _sortTweenDuration = 0.2f;

    private readonly List<NumberCardView> _views = new List<NumberCardView>();
    private readonly List<CardInstance> _cards = new List<CardInstance>();

    public event Action<CardInstance, PointerEventData> NumberCardDropped;

    public void ShowCards(IReadOnlyList<CardInstance> cards)
    {
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
        SortCards();
    }

    public void SortCards()
    {
        int count = _cards.Count;

        for (int i = 0; i < _views.Count; i++)
        {
            bool active = i < count;
            _views[i].gameObject.SetActive(active);
        }

        if (count == 0)
        {
            return;
        }

        float angleStep = GetAngleStep(count);
        float startAngle = -angleStep * (count - 1) * 0.5f;

        for (int i = 0; i < count; i++)
        {
            RectTransform cardTransform = _views[i].transform as RectTransform;
            float angle = startAngle + angleStep * i;
            Vector2 position = GetFanPosition(angle);

            _views[i].ReturnToLayoutParent();
            _views[i].transform.SetSiblingIndex(i);

            cardTransform.DOKill();
            cardTransform.DOAnchorPos(position, _sortTweenDuration).SetEase(Ease.OutCubic);
            cardTransform.DOLocalRotate(new Vector3(0f, 0f, -angle), _sortTweenDuration).SetEase(Ease.OutCubic);
            cardTransform.DOScale(new Vector3(_cardScale, _cardScale, 1f), _sortTweenDuration).SetEase(Ease.OutCubic);
        }
    }

    public void ReturnDraggedCard(NumberCardView cardView)
    {
        if (cardView == null)
        {
            return;
        }

        cardView.ReturnToLayoutParent();
        SortCards();
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
            Debug.LogError("NumberCardHandView needs a card prefab.");
            return;
        }

        RectTransform parent = _cardRoot;
        if (parent == null)
        {
            parent = transform as RectTransform;
        }

        Canvas canvas = _canvas;
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        RectTransform dragRoot = _dragRoot;
        if (dragRoot == null && canvas != null)
        {
            dragRoot = canvas.transform as RectTransform;
        }

        NumberCardView view = Instantiate(_cardPrefab, parent);
        view.Setup(canvas, dragRoot);
        view.DragEnded += OnCardDragEnded;
        _views.Add(view);
    }

    private void BindCards()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            _views[i].SetCard(_cards[i]);
        }
    }

    private float GetAngleStep(int count)
    {
        if (count <= 1)
        {
            return 0f;
        }

        float angleStep = _maxFanAngle / (count - 1);
        if (angleStep > _maxAngleStep)
        {
            angleStep = _maxAngleStep;
        }

        return angleStep;
    }

    private Vector2 GetFanPosition(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float x = Mathf.Sin(radians) * _radius;
        float y = Mathf.Cos(radians) * _radius - _radius + _verticalOffset;

        return new Vector2(x, y);
    }

    private void OnCardDragEnded(NumberCardView view, PointerEventData eventData)
    {
        if (view == null)
        {
            return;
        }

        if (NumberCardDropped != null)
        {
            NumberCardDropped(view.Card, eventData);
        }
    }
}
