using System;
using System.Collections.Generic;
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

        public event Action<string> ServeOrderClicked;

        private void OnDestroy()
        {
            for (var i = 0; i < _slots.Length; i++)
            {
                _slots[i].Hide();
            }
        }

        public void SetOrders(IReadOnlyList<OrderRuntimeModel> orders)
        {
            for (var i = 0; i < _slots.Length; i++)
            {
                if (i < orders.Count)
                {
                    _slots[i].Show(orders[i], OnServeOrderClicked);
                }
                else
                {
                    _slots[i].Hide();
                }
            }
        }

        public void PlayOrderCompleted(ServeResult result)
        {
            SetMessage("서빙 완료: 별 " + result.Star + "개 / +" + result.FinalScore);
        }

        public void PlayOrderExpired(string orderInstanceId)
        {
            SetMessage("주문이 만료되었습니다.");
        }

        private void OnServeOrderClicked(string orderInstanceId)
        {
            ServeOrderClicked?.Invoke(orderInstanceId);
        }

        private void SetMessage(string message)
        {
            if (_messageText != null)
            {
                _messageText.text = message;
            }
        }
    }
}
