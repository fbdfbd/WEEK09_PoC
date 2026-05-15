using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class IndexSelectorView : MonoBehaviour
{
    [SerializeField] private Button[] _buttons;
    [SerializeField] private TextMeshProUGUI[] _labels;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _selectedColor = new(1f, 0.72f, 0.18f);

    public event Action<int> OnIndexClicked;

    private void Awake()
    {
        AutoBindChildButtons();

        for (var i = 0; i < _buttons.Length; i++)
        {
            var index = i;
            _buttons[i].onClick.AddListener(() => OnIndexClicked?.Invoke(index));
        }
    }

    private void AutoBindChildButtons()
    {
        var childButtons = GetComponentsInChildren<Button>(true);
        var mergedButtons = new List<Button>(_buttons ?? Array.Empty<Button>());

        foreach (var button in childButtons)
        {
            if (!mergedButtons.Contains(button))
                mergedButtons.Add(button);
        }

        _buttons = mergedButtons.ToArray();

        var labels = new List<TextMeshProUGUI>();
        foreach (var button in _buttons)
        {
            var label = button.GetComponentInChildren<TextMeshProUGUI>(true);
            labels.Add(label);
        }

        _labels = labels.ToArray();
    }

    public void SetCount(int count)
    {
        for (var i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].gameObject.SetActive(i < count);

            if (i < _labels.Length && _labels[i] != null)
                _labels[i].text = (i + 1).ToString();
        }
    }

    public void SetSelectedIndex(int selectedIndex)
    {
        for (var i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i].targetGraphic != null)
                _buttons[i].targetGraphic.color = i == selectedIndex ? _selectedColor : _normalColor;
        }
    }

    public void SetInteractable(int index, bool value)
    {
        if (index < 0 || index >= _buttons.Length)
            return;

        _buttons[index].gameObject.SetActive(value);
    }
}
