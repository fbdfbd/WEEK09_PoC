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

    private readonly List<Button> _buttonPool = new();
    private readonly List<TextMeshProUGUI> _labelPool = new();

    private Button _templateButton;
    private Transform _buttonParent;

    public event Action<int> OnIndexClicked;

    private void Awake()
    {
        CacheSceneButtons();
        SetCount(0);
    }

    private void CacheSceneButtons()
    {
        var buttons = new List<Button>();

        var childButtons = GetComponentsInChildren<Button>(true);
        AddButtons(buttons, childButtons);
        AddButtons(buttons, _buttons);

        if (buttons.Count == 0)
        {
            enabled = false;
            return;
        }

        _templateButton = buttons[0];
        _buttonParent = _templateButton.transform.parent;

        foreach (var button in buttons)
            AddButtonToPool(button);

        RefreshSerializedCache();
    }

    private void AddButtons(List<Button> target, IEnumerable<Button> source)
    {
        if (source == null)
            return;

        foreach (var button in source)
        {
            if (button != null && button.transform.IsChildOf(transform) && !target.Contains(button))
                target.Add(button);
        }
    }

    private void AddButtonToPool(Button button)
    {
        var index = _buttonPool.Count;

        _buttonPool.Add(button);
        _labelPool.Add(button.GetComponentInChildren<TextMeshProUGUI>(true));

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnIndexClicked?.Invoke(index));
    }

    private void EnsureButtonCount(int count)
    {
        if (_templateButton == null || _buttonParent == null)
            return;

        while (_buttonPool.Count < count)
        {
            var button = Instantiate(_templateButton, _buttonParent);
            button.name = $"{_templateButton.name}_{_buttonPool.Count + 1}";
            AddButtonToPool(button);
        }

        RefreshSerializedCache();
    }

    private void RefreshSerializedCache()
    {
        _buttons = _buttonPool.ToArray();
        _labels = _labelPool.ToArray();
    }

    public void SetCount(int count)
    {
        count = Mathf.Max(0, count);
        EnsureButtonCount(count);

        for (var i = 0; i < _buttonPool.Count; i++)
        {
            _buttonPool[i].gameObject.SetActive(i < count);

            if (_labelPool[i] != null)
                _labelPool[i].text = (i + 1).ToString();
        }
    }

    public void SetSelectedIndex(int selectedIndex)
    {
        for (var i = 0; i < _buttonPool.Count; i++)
        {
            if (_buttonPool[i].targetGraphic != null)
                _buttonPool[i].targetGraphic.color = i == selectedIndex ? _selectedColor : _normalColor;
        }
    }

    public void SetInteractable(int index, bool value)
    {
        if (index < 0 || index >= _buttonPool.Count)
            return;

        _buttonPool[index].interactable = value;
    }
}
