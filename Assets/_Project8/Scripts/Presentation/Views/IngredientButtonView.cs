using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project8.Presentation.Views
{
    [Serializable]
    public sealed class IngredientButtonView
    {
        [SerializeField] private string _ingredientId;
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;

        public string IngredientId { get { return _ingredientId; } }

        public void SetLabel(string text)
        {
            if (_label != null)
            {
                _label.text = text;
            }
        }

        public void AddListener(Action<string> listener)
        {
            if (_button == null)
            {
                return;
            }

            _button.onClick.AddListener(() => listener.Invoke(_ingredientId));
        }

        public void RemoveAllListeners()
        {
            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
            }
        }
    }
}
