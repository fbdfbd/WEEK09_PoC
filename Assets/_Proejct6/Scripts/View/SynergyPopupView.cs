using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SynergyPopupView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panel;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private float _showDuration = 0.2f;
    [SerializeField] private float _stayDuration = 0.8f;
    [SerializeField] private float _hideDuration = 0.25f;

    private readonly Queue<SynergyPopupRequest> _queue = new Queue<SynergyPopupRequest>();
    private bool _isPlaying;

    private void Awake()
    {
        HideImmediately();
    }

    public void Enqueue(SynergyPopupRequest request)
    {
        if (request == null)
        {
            return;
        }

        _queue.Enqueue(request);

        if (_isPlaying == false)
        {
            PlayNext();
        }
    }

    private void PlayNext()
    {
        if (_queue.Count == 0)
        {
            _isPlaying = false;
            return;
        }

        _isPlaying = true;
        SynergyPopupRequest request = _queue.Dequeue();
        ShowRequest(request);
    }

    private void ShowRequest(SynergyPopupRequest request)
    {
        if (_titleText != null)
        {
            _titleText.text = request.Title;
        }

        if (_descriptionText != null)
        {
            _descriptionText.text = request.Description;
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0f;
        }

        if (_panel != null)
        {
            _panel.DOKill();
            _panel.localScale = new Vector3(0.85f, 0.85f, 1f);
        }

        Sequence sequence = DOTween.Sequence();

        if (_canvasGroup != null)
        {
            sequence.Join(_canvasGroup.DOFade(1f, _showDuration));
        }

        if (_panel != null)
        {
            sequence.Join(_panel.DOScale(new Vector3(1.08f, 1.08f, 1f), _showDuration).SetEase(Ease.OutBack));
            sequence.Append(_panel.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutCubic));
        }

        sequence.AppendInterval(_stayDuration);

        if (_canvasGroup != null)
        {
            sequence.Append(_canvasGroup.DOFade(0f, _hideDuration));
        }

        if (_panel != null)
        {
            sequence.Join(_panel.DOScale(new Vector3(0.9f, 0.9f, 1f), _hideDuration).SetEase(Ease.InCubic));
        }

        sequence.OnComplete(PlayNext);
    }

    private void HideImmediately()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
        }

        if (_panel != null)
        {
            _panel.localScale = new Vector3(0.9f, 0.9f, 1f);
        }
    }
}
