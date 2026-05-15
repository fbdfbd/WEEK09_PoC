using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class RequestView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _bodyText;
    [SerializeField] private TextMeshProUGUI _summaryText;

    [SerializeField] private Button _receivedButton;
    [SerializeField] private Button _correctionRequiredButton;
    [SerializeField] private Button _pendingButton;
    [SerializeField] private Button _rejectedButton;

    [SerializeField] private GameObject _requestDecisionPanel;
    [SerializeField] private GameObject _agencyAssignmentPanel;

    [SerializeField] private Button _internalOrderMinistryButton;
    [SerializeField] private Button _militarySecurityDirectorateButton;
    [SerializeField] private Button _healthAuthorityButton;

    [SerializeField] private GameObject _interationOptionTag;
    [SerializeField] private GameObject _agencyAssignmentTag;

    [SerializeField] private TextMeshProUGUI _interationOptionTagText;
    [SerializeField] private TextMeshProUGUI _agencyAssignmentTagText;

    public event Action OnReceivedClicked;
    public event Action OnCorrectionRequiredClicked;
    public event Action OnPendingClicked;
    public event Action OnRejectedClicked;
    public event Action<string> OnAgencyClicked;

    private void Awake()
    {
        _receivedButton.onClick.AddListener(() => OnReceivedClicked?.Invoke());
        _correctionRequiredButton.onClick.AddListener(() => OnCorrectionRequiredClicked?.Invoke());
        _pendingButton.onClick.AddListener(() => OnPendingClicked?.Invoke());
        _rejectedButton.onClick.AddListener(() => OnRejectedClicked?.Invoke());

        _internalOrderMinistryButton.onClick.AddListener(() => OnAgencyClicked?.Invoke("test1"));
        _militarySecurityDirectorateButton.onClick.AddListener(() => OnAgencyClicked?.Invoke("test2"));
        _healthAuthorityButton.onClick.AddListener(() => OnAgencyClicked?.Invoke("test3"));
    }

    public void SetRequest(string title, string body, string summary)
    {
        _titleText.text = title;
        _bodyText.text = body;
        _summaryText.text = summary;
    }

    public void SetButtonsInteractable(bool value)
    {
        _receivedButton.interactable = value;
        _correctionRequiredButton.interactable = value;
        _pendingButton.interactable = value;
        _rejectedButton.interactable = value;
    }

    public void SetRequestDecisionPanelShow(bool value)
    {
        _requestDecisionPanel.SetActive(value);
    }

    public void SetAgencyAssignmentPanelShow(bool value)
    {
        _agencyAssignmentPanel.SetActive(value);
    }

    public void SetAgencyTagText(string text)
    {
        _agencyAssignmentTagText.text = text;
    }

    public void SetInteractionTagText(string text)
    {
        _interationOptionTagText.text = text;
    }

    public void SetAgencyTagShow(bool value)
    {
        _agencyAssignmentTag.SetActive(value);
    }

    public void SetInteractionTagShow(bool value)
    {
        _interationOptionTag.SetActive(value);
    }
}
