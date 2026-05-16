using System;
using System.Collections.Generic;
using TMPro;
using Project8.Domain.Model;
using UnityEngine;

namespace Project8.Presentation.Views
{
    public sealed class IngredientPanelView : MonoBehaviour, IIngredientPanelView
    {
        [SerializeField] private IngredientButtonView[] _buttons;
        [SerializeField] private TMP_Text _messageText;

        public event Action<string> IngredientClicked;

        private void Awake()
        {
            for (var i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].AddListener(OnIngredientClicked);
            }
        }

        private void OnDestroy()
        {
            for (var i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].RemoveAllListeners();
            }
        }

        public void SetIngredients(IReadOnlyList<IngredientRuntimeModel> ingredients)
        {
            for (var i = 0; i < _buttons.Length; i++)
            {
                var ingredient = FindIngredient(ingredients, _buttons[i].IngredientId);

                if (ingredient != null)
                {
                    _buttons[i].SetLabel(ingredient.DisplayName);
                }
            }
        }

        public void PlayIngredientUsed(string ingredientId)
        {
            SetMessage("재료를 넣었습니다: " + ingredientId);
        }

        public void PlayInvalidAction(string message)
        {
            SetMessage(message);
        }

        private void OnIngredientClicked(string ingredientId)
        {
            IngredientClicked?.Invoke(ingredientId);
        }

        private void SetMessage(string message)
        {
            if (_messageText != null)
            {
                _messageText.text = message;
            }
        }

        private static IngredientRuntimeModel FindIngredient(
            IReadOnlyList<IngredientRuntimeModel> ingredients,
            string ingredientId)
        {
            for (var i = 0; i < ingredients.Count; i++)
            {
                if (ingredients[i].Id == ingredientId)
                {
                    return ingredients[i];
                }
            }

            return null;
        }
    }
}
