using System;
using TMPro;
using Project8.Domain.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Project8.Presentation.Views
{
    [Serializable]
    public sealed class OrderSlotView
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _tasteText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Button _serveButton;

        private string _orderInstanceId;

        public void Show(OrderRuntimeModel order, Action<string> serveClicked)
        {
            _orderInstanceId = order.InstanceId;

            if (_root != null)
            {
                _root.SetActive(true);
            }

            SetText(_nameText, order.DisplayName);
            SetText(_tasteText, CreateTasteText(order));
            SetText(_timeText, Mathf.CeilToInt(order.RemainingPatienceSeconds) + "s");

            if (_serveButton != null)
            {
                _serveButton.onClick.RemoveAllListeners();
                _serveButton.onClick.AddListener(() => serveClicked.Invoke(_orderInstanceId));
            }
        }

        public void Hide()
        {
            _orderInstanceId = string.Empty;

            if (_root != null)
            {
                _root.SetActive(false);
            }

            if (_serveButton != null)
            {
                _serveButton.onClick.RemoveAllListeners();
            }
        }

        private static string CreateTasteText(OrderRuntimeModel order)
        {
            return "매 " + FormatRange(order.SpicyRange.Min, order.SpicyRange.Max)
                + " / 단 " + FormatRange(order.SweetRange.Min, order.SweetRange.Max)
                + " / 농 " + FormatRange(order.ThickRange.Min, order.ThickRange.Max);
        }

        private static string FormatRange(float min, float max)
        {
            return Mathf.RoundToInt(min) + "-" + Mathf.RoundToInt(max);
        }

        private static void SetText(TMP_Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
