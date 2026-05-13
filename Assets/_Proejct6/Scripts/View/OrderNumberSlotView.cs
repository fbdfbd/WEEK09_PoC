using TMPro;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OrderNumberSlotView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text _numberText;

    public event Action Clicked;

    public void Show(CardInstance numberCard)
    {
        if (_numberText == null)
        {
            return;
        }

        NumCardDefinition definition = BattleState.GetNumDefinition(numberCard);
        if (definition == null)
        {
            _numberText.text = string.Empty;
            return;
        }

        _numberText.text = definition.Number.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Clicked != null)
        {
            Clicked();
        }
    }
}
