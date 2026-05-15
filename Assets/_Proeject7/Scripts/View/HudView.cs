using TMPro;
using UnityEngine;

public sealed class HudView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _phaseText;

    public void SetDay(int day)
    {
        _dayText.text = $"DAY {day}";
    }

    public void SetPhase(string phaseText)
    {
        _phaseText.text = phaseText;
    }
}