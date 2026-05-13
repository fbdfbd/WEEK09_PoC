using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleLogView : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _contentRoot;
    [SerializeField] private TMP_Text _linePrefab;
    [SerializeField] private bool _autoScrollToBottom = true;

    private readonly List<TMP_Text> _lineViews = new List<TMP_Text>();
    private int _shownCount;

    public void Show(IReadOnlyList<BattleLogEntry> entries)
    {
        if (entries == null || entries.Count == 0)
        {
            return;
        }

        for (int i = _shownCount; i < entries.Count; i++)
        {
            AddLine(entries[i]);
        }

        _shownCount = entries.Count;

        if (_autoScrollToBottom)
        {
            Canvas.ForceUpdateCanvases();
            ScrollToBottom();
        }
    }

    public void Clear()
    {
        for (int i = 0; i < _lineViews.Count; i++)
        {
            if (_lineViews[i] != null)
            {
                Destroy(_lineViews[i].gameObject);
            }
        }

        _lineViews.Clear();
        _shownCount = 0;
    }

    private void AddLine(BattleLogEntry entry)
    {
        if (entry == null || _linePrefab == null)
        {
            return;
        }

        RectTransform parent = _contentRoot;
        if (parent == null)
        {
            parent = transform as RectTransform;
        }

        TMP_Text lineView = Instantiate(_linePrefab, parent);
        lineView.text = entry.Message;
        lineView.gameObject.SetActive(true);
        _lineViews.Add(lineView);
    }

    private void ScrollToBottom()
    {
        if (_scrollRect == null)
        {
            return;
        }

        _scrollRect.verticalNormalizedPosition = 0f;
    }
}
