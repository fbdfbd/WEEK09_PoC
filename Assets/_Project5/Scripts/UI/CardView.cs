using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private TMP_Text numberText;

    private CardInstance card;

    public event Action<CardView> Clicked;

    public CardInstance Card
    {
        get { return card; }
    }

    public void SetCard(CardInstance newCard)
    {
        card = newCard;
        RefreshTexts();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Clicked != null)
        {
            Clicked(this);
        }
    }

    private void RefreshTexts()
    {
        if (displayNameText != null)
        {
            displayNameText.text = GetDisplayName();
        }

        if (numberText != null)
        {
            numberText.text = GetNumberText();
        }
    }

    private string GetDisplayName()
    {
        if (card == null || card.Definition == null)
        {
            return string.Empty;
        }

        if (card.Definition is NumCardDefinition)
        {
            return string.Empty;
        }

        return card.Definition.DisplayName;
    }

    private string GetNumberText()
    {
        NumCardDefinition numberDefinition = null;
        if (card != null)
        {
            numberDefinition = card.Definition as NumCardDefinition;
        }

        if (numberDefinition == null)
        {
            return string.Empty;
        }

        return numberDefinition.Value.ToString();
    }
}
