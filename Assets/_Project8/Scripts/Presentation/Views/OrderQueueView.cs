using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Project8.Domain.Data;
using Project8.Domain.Model;
using UnityEngine;

namespace Project8.Presentation.Views
{
    public sealed class OrderQueueView : MonoBehaviour, IOrderQueueView
    {
        [SerializeField] private OrderSlotView[] _slots;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private TMP_Text _waitingOrderCountText;
        [SerializeField] private float _messagePunchScale = 0.35f;
        [SerializeField] private float _messagePunchDuration = 0.45f;
        [SerializeField] private Color _normalMessageColor = Color.white;
        [SerializeField] private Color _successMessageColor = new Color(1f, 0.86f, 0.18f, 1f);
        [SerializeField] private Color _failMessageColor = new Color(1f, 0.35f, 0.28f, 1f);

        private Sequence _messageSequence;
        private Vector3 _messageInitialScale = Vector3.one;

        public event Action<string> ServeOrderClicked;

        private void Awake()
        {
            if (_messageText == null)
            {
                return;
            }

            _messageInitialScale = _messageText.rectTransform.localScale;
            _messageText.color = _normalMessageColor;
        }

        private void OnDestroy()
        {
            _messageSequence?.Kill();

            for (var i = 0; i < _slots.Length; i++)
            {
                _slots[i].Hide();
            }
        }

        public void SetOrders(IReadOnlyList<OrderRuntimeModel> orders)
        {
            var visibleOrderCount = Mathf.Min(_slots.Length, orders.Count);

            for (var i = 0; i < _slots.Length; i++)
            {
                if (i < visibleOrderCount)
                {
                    _slots[i].Show(orders[i], OnServeOrderClicked);
                }
                else
                {
                    _slots[i].Hide();
                }
            }

            SetWaitingOrderCount(orders.Count - visibleOrderCount);
        }

        public void PlayOrderCompleted(ServeResult result)
        {
            SetMessage(
                "서빙 완료! 별 " + result.Star + "개 / +" + result.FinalScore,
                _successMessageColor);
        }

        public void PlayOrderExpired(string orderInstanceId)
        {
            SetMessage("주문이 만료되었습니다.", _failMessageColor);
        }

        private void OnServeOrderClicked(string orderInstanceId)
        {
            ServeOrderClicked?.Invoke(orderInstanceId);
        }

        private void SetMessage(string message, Color color)
        {
            if (_messageText == null)
            {
                return;
            }

            _messageText.text = message;
            _messageText.color = color;
            PlayMessageTween();
        }

        private void SetWaitingOrderCount(int waitingOrderCount)
        {
            if (_waitingOrderCountText == null)
            {
                return;
            }

            if (waitingOrderCount <= 0)
            {
                _waitingOrderCountText.text = string.Empty;
                return;
            }

            _waitingOrderCountText.text = "대기 주문 " + waitingOrderCount + "개";
        }

        private void PlayMessageTween()
        {
            var rectTransform = _messageText.rectTransform;

            _messageSequence?.Kill();
            rectTransform.localScale = _messageInitialScale;

            _messageSequence = DOTween.Sequence()
                .Append(rectTransform.DOPunchScale(
                    Vector3.one * _messagePunchScale,
                    _messagePunchDuration,
                    8,
                    0.65f))
                .AppendInterval(0.65f)
                .Append(_messageText.DOColor(_normalMessageColor, 0.25f))
                .SetTarget(this);
        }
    }
}
