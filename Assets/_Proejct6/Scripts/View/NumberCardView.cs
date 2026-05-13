using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumberCardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TMP_Text _numberText;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _dragRotateDuration = 0.12f;

    private Canvas _canvas;
    private RectTransform _rectTransform;
    private RectTransform _dragRoot;
    private Transform _layoutParent;
    private CardInstance _card;

    public event Action<NumberCardView, PointerEventData> DragEnded;

    public CardInstance Card => _card;

    public void Setup(Canvas canvas, RectTransform dragRoot)
    {
        _canvas = canvas;
        _dragRoot = dragRoot;
        _rectTransform = transform as RectTransform;

        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void SetCard(CardInstance card)
    {
        _card = card;
        RefreshText();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _layoutParent = transform.parent;

        if (_dragRoot != null)
        {
            transform.SetParent(_dragRoot, true);
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = false;
        }

        transform.SetAsLastSibling();
        transform.DOKill();
        transform.DOLocalRotate(Vector3.zero, _dragRotateDuration).SetEase(Ease.OutCubic);
        MoveToPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveToPointer(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
        }

        if (DragEnded != null)
        {
            DragEnded(this, eventData);
        }
    }

    public void ReturnToLayoutParent()
    {
        if (_layoutParent != null)
        {
            transform.SetParent(_layoutParent, true);
        }
    }

    private void RefreshText()
    {
        if (_numberText == null)
        {
            return;
        }

        NumCardDefinition definition = BattleState.GetNumDefinition(_card);
        if (definition == null)
        {
            _numberText.text = string.Empty;
            return;
        }

        _numberText.text = definition.Number.ToString();
    }

    private void MoveToPointer(PointerEventData eventData)
    {
        if (_canvas == null || _rectTransform == null)
        {
            return;
        }

        RectTransform targetTransform = _dragRoot;
        if (targetTransform == null)
        {
            targetTransform = _canvas.transform as RectTransform;
        }

        Vector2 localPoint;

        bool hasPoint = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            targetTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        if (hasPoint)
        {
            _rectTransform.anchoredPosition = localPoint;
        }
    }
}
