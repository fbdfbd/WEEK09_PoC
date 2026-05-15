using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class FlowControlView : MonoBehaviour
{
    [SerializeField] private Button _nextButton;
    [SerializeField] private TextMeshProUGUI _nextButtonText;

    public event Action OnNextClicked;

    private void Awake()
    {
        _nextButton.onClick.AddListener(() => OnNextClicked?.Invoke());
    }

    public void SetNextText(string text)
    {
        _nextButtonText.text = text;
    }

    public void SetNextInteractable(bool value)
    {
        _nextButton.interactable = value;
    }
}