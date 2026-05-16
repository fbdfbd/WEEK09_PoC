using System;
using System.Collections.Generic;
using Project8.Domain.Model;

namespace Project8.Presentation.Views
{
    public interface IIngredientPanelView
    {
        event Action<string> IngredientClicked;

        void SetIngredients(IReadOnlyList<IngredientRuntimeModel> ingredients);
        void PlayIngredientUsed(string ingredientId);
        void PlayInvalidAction(string message);
    }
}
