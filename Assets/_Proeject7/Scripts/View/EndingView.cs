using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class EndingView : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _playerOutcomeText;
    [SerializeField] private TextMeshProUGUI _achievementText;
    [SerializeField] private TextMeshProUGUI _trustText;
    [SerializeField] private TextMeshProUGUI _agencyRelationsText;
    [SerializeField] private TextMeshProUGUI _incidentsText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _quitButton;

    public event Action OnRestartClicked;
    public event Action OnQuitClicked;

    private void Awake()
    {
        if (_restartButton != null)
            _restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());

        if (_quitButton != null)
            _quitButton.onClick.AddListener(() => OnQuitClicked?.Invoke());

        SetShow(false);
    }

    public void SetShow(bool value)
    {
        if (_panel != null)
            _panel.SetActive(value);
        else
            gameObject.SetActive(value);
    }

    public void SetReport(EndingReport report)
    {
        if (_titleText != null)
            _titleText.text = report.Title;

        if (_playerOutcomeText != null)
            _playerOutcomeText.text = report.PlayerOutcomeText;

        if (_achievementText != null)
            _achievementText.text = report.AchievementText;

        if (_trustText != null)
            _trustText.text = report.TrustText;

        if (_agencyRelationsText != null)
            _agencyRelationsText.text = report.AgencyRelationsText;

        if (_incidentsText != null)
            _incidentsText.text = report.IncidentsText;
    }
}
