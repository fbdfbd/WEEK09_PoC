using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class IntroView : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _advanceButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private float _secondsPerCharacter = 0.035f;

    private Tween _typingTween;
    private string _completedText = string.Empty;
    private string _currentLine = string.Empty;

    public event Action OnAdvanceClicked;
    public event Action OnStartClicked;

    public bool IsTyping => _typingTween != null && _typingTween.IsActive() && _typingTween.IsPlaying();

    private void Awake()
    {
        if (_advanceButton != null)
            _advanceButton.onClick.AddListener(() => OnAdvanceClicked?.Invoke());

        if (_startButton != null)
            _startButton.onClick.AddListener(() => OnStartClicked?.Invoke());
    }

    public void SetShow(bool value)
    {
        if (_root != null)
            _root.SetActive(value);
    }

    public void SetStartInteractable(bool value)
    {
        if (_startButton != null)
            _startButton.interactable = value;
    }

    public void ClearText()
    {
        KillTyping();

        if (_text != null)
            _text.text = string.Empty;

        _completedText = string.Empty;
        _currentLine = string.Empty;
    }

    public void PlayLine(string line, Action onComplete)
    {
        KillTyping();

        if (_text == null)
        {
            onComplete?.Invoke();
            return;
        }

        _currentLine = line;
        var prefix = string.IsNullOrEmpty(_completedText)
            ? string.Empty
            : _completedText + "\n";

        _text.text = prefix;

        var duration = Mathf.Max(0.01f, line.Length * _secondsPerCharacter);
        _typingTween = DOTween
            .To(
                () => string.Empty,
                value => _text.text = prefix + value,
                line,
                duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _completedText = prefix + _currentLine;
                _text.text = _completedText;
                _currentLine = string.Empty;
                _typingTween = null;
                onComplete?.Invoke();
            });
    }

    public void CompleteTyping()
    {
        if (_typingTween == null || !_typingTween.IsActive())
            return;

        _typingTween.Complete();
    }

    private void KillTyping()
    {
        if (_typingTween == null)
            return;

        _typingTween.Kill();
        _typingTween = null;
    }

    private void OnDestroy()
    {
        KillTyping();
    }
}
