using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillCardView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _requirementText;
    [SerializeField] private TMP_Text _equippedNumbersText;

    private CardInstance _card;

    public event Action<CardInstance> Clicked;

    public CardInstance Card => _card;

    public void SetCard(CardInstance card)
    {
        SetCard(card, null);
    }

    public void SetCard(CardInstance card, SkillSlotState slot)
    {
        _card = card;
        RefreshTexts(slot);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Clicked != null)
        {
            Clicked(_card);
        }
    }

    private void RefreshTexts(SkillSlotState slot)
    {
        SkillCardDefinition definition = BattleState.GetSkillDefinition(_card);

        if (_nameText != null)
        {
            _nameText.text = GetName(definition);
        }

        if (_descriptionText != null)
        {
            _descriptionText.text = GetDescription(definition);
        }

        if (_requirementText != null)
        {
            _requirementText.text = GetRequirementText(definition);
        }

        if (_equippedNumbersText != null)
        {
            _equippedNumbersText.text = GetEquippedNumbersText(slot);
        }
    }

    private string GetName(SkillCardDefinition definition)
    {
        if (definition == null)
        {
            return string.Empty;
        }

        return definition.DisplayName;
    }

    private string GetDescription(SkillCardDefinition definition)
    {
        if (definition == null)
        {
            return string.Empty;
        }

        return definition.Description;
    }

    private string GetRequirementText(SkillCardDefinition definition)
    {
        if (definition == null)
        {
            return string.Empty;
        }

        return "x" + definition.RequiredCount;
    }

    private string GetEquippedNumbersText(SkillSlotState slot)
    {
        if (slot == null || slot.NumberCards.Count == 0)
        {
            return string.Empty;
        }

        string text = string.Empty;

        for (int i = 0; i < slot.NumberCards.Count; i++)
        {
            NumCardDefinition number = BattleState.GetNumDefinition(slot.NumberCards[i]);
            if (number == null)
            {
                continue;
            }

            if (text.Length > 0)
            {
                text += ", ";
            }

            text += number.Number.ToString();
        }

        return text;
    }
}
