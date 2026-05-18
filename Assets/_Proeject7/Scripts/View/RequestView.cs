using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public sealed class RejectReasonButtonBinding
{
    [SerializeField] private RejectReason _reason;
    [SerializeField] private Button _button;

    public RejectReason Reason => _reason;
    public Button Button => _button;
}

[Serializable]
public sealed class AgencyButtonBinding
{
    [SerializeField] private string _agencyId;
    [SerializeField] private Button _button;

    public string AgencyId => _agencyId;
    public Button Button => _button;
}

public sealed class RequestView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _bodyText;
    [SerializeField] private TextMeshProUGUI _summaryText;
    [SerializeField] private TextMeshProUGUI _deadlineText;

    [SerializeField] private Button _receivedButton;
    [SerializeField] private Button _correctionRequiredButton;
    [SerializeField] private Button _pendingButton;
    [SerializeField] private Button _rejectedButton;
    [SerializeField] private GameObject _rejectReasonPanel;
    [SerializeField] private RejectReasonButtonBinding[] _rejectReasonButtons;

    [SerializeField] private GameObject _requestDecisionPanel;
    [SerializeField] private GameObject _agencyAssignmentPanel;

    [SerializeField] private Button _internalOrderMinistryButton;
    [SerializeField] private Button _militarySecurityDirectorateButton;
    [SerializeField] private Button _healthAuthorityButton;
    [SerializeField] private AgencyButtonBinding[] _agencyButtons;

    [SerializeField] private GameObject _interationOptionTag;
    [SerializeField] private GameObject _agencyAssignmentTag;

    [SerializeField] private TextMeshProUGUI _interationOptionTagText;
    [SerializeField] private TextMeshProUGUI _agencyAssignmentTagText;

    public event Action OnReceivedClicked;
    public event Action OnCorrectionRequiredClicked;
    public event Action OnPendingClicked;
    public event Action OnRejectedClicked;
    public event Action<RejectReason> OnRejectReasonClicked;
    public event Action<string> OnAgencyClicked;

    private void Awake()
    {
        ConfigureDecisionTooltips();

        _receivedButton.onClick.AddListener(() => OnReceivedClicked?.Invoke());
        _correctionRequiredButton.onClick.AddListener(() => OnCorrectionRequiredClicked?.Invoke());
        _pendingButton.onClick.AddListener(() => OnPendingClicked?.Invoke());
        _rejectedButton.onClick.AddListener(() => OnRejectedClicked?.Invoke());

        if (_rejectReasonButtons != null)
        {
            foreach (var binding in _rejectReasonButtons)
            {
                if (binding?.Button == null)
                    continue;

                var reason = binding.Reason;
                binding.Button.onClick.AddListener(() => OnRejectReasonClicked?.Invoke(reason));
            }
        }

        ConfigureAgencyButtons();
    }

    public void SetAgencyTooltipText(string agencyId, string text)
    {
        var button = GetAgencyButton(agencyId);
        if (button == null)
            return;

        EnsureTooltipTrigger(button).SetOverrideText(text);
    }

    public void SetRequest(string title, string body, string summary)
    {
        _titleText.text = title;
        _bodyText.text = body;
        _summaryText.text = summary;
    }

    public void SetDeadlineText(string text)
    {
        if (_deadlineText != null)
            _deadlineText.text = text;
    }

    public void SetButtonsInteractable(bool value)
    {
        _receivedButton.interactable = value;
        _correctionRequiredButton.interactable = value;
        _pendingButton.interactable = value;
        _rejectedButton.interactable = value;
    }

    public void SetPendingButtonInteractable(bool value)
    {
        _pendingButton.interactable = value;
    }

    public void SetRejectReasonPanelShow(bool value)
    {
        if (_rejectReasonPanel != null)
            _rejectReasonPanel.SetActive(value);
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

    private void ConfigureDecisionTooltips()
    {
        SetButtonTooltipKey(_receivedButton, TooltipKey.AcceptRequest);
        SetButtonTooltipKey(_correctionRequiredButton, TooltipKey.SupplementRequired);
        SetButtonTooltipKey(_pendingButton, TooltipKey.DeferRequest);
        SetButtonTooltipKey(_rejectedButton, TooltipKey.RejectRequest);

        if (_rejectReasonButtons == null)
            return;

        foreach (var binding in _rejectReasonButtons)
        {
            if (binding?.Button == null)
                continue;

            SetButtonTooltipKey(binding.Button, GetRejectReasonTooltipKey(binding.Reason));
        }
    }

    private Button GetAgencyButton(string agencyId)
    {
        if (_agencyButtons != null)
        {
            foreach (var binding in _agencyButtons)
            {
                if (binding?.Button == null || binding.AgencyId != agencyId)
                    continue;

                return binding.Button;
            }
        }

        return agencyId switch
        {
            "test1" => _internalOrderMinistryButton,
            "test2" => _militarySecurityDirectorateButton,
            "test3" => _healthAuthorityButton,
            _ => null
        };
    }

    private void ConfigureAgencyButtons()
    {
        var hasAgencyBindings = _agencyButtons != null && _agencyButtons.Length > 0;

        if (hasAgencyBindings)
        {
            foreach (var binding in _agencyButtons)
            {
                if (binding?.Button == null || string.IsNullOrEmpty(binding.AgencyId))
                    continue;

                var agencyId = binding.AgencyId;
                binding.Button.onClick.AddListener(() => OnAgencyClicked?.Invoke(agencyId));
            }

            return;
        }

        _internalOrderMinistryButton.onClick.AddListener(() => OnAgencyClicked?.Invoke("test1"));
        _militarySecurityDirectorateButton.onClick.AddListener(() => OnAgencyClicked?.Invoke("test2"));
        _healthAuthorityButton.onClick.AddListener(() => OnAgencyClicked?.Invoke("test3"));
    }

    private static TooltipTrigger EnsureTooltipTrigger(Button button)
    {
        if (button == null)
            return null;

        if (!button.TryGetComponent<TooltipTrigger>(out var trigger))
            trigger = button.gameObject.AddComponent<TooltipTrigger>();

        return trigger;
    }

    private static void SetButtonTooltipKey(Button button, TooltipKey key)
    {
        var trigger = EnsureTooltipTrigger(button);
        if (trigger != null)
            trigger.SetKey(key);
    }

    private static TooltipKey GetRejectReasonTooltipKey(RejectReason reason)
    {
        return reason switch
        {
            RejectReason.PersonalInformation => TooltipKey.RejectPersonalInformation,
            RejectReason.NationalSecurity => TooltipKey.RejectNationalSecurity,
            _ => TooltipKey.None
        };
    }
}
