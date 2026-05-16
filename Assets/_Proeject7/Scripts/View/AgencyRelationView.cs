using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public sealed class AgencyRelationBinding
{
    [SerializeField] private string _agencyId;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _relationText;
    [SerializeField] private Slider _relationSlider;

    public string AgencyId => _agencyId;

    public void Set(string agencyName, int relation)
    {
        if (_nameText != null)
            _nameText.text = agencyName;

        if (_relationText != null)
            _relationText.text = relation.ToString();

        if (_relationSlider != null)
        {
            _relationSlider.minValue = AgencyData.MinRelation;
            _relationSlider.maxValue = AgencyData.MaxRelation;
            _relationSlider.value = relation;
        }
    }
}

public sealed class AgencyRelationView : MonoBehaviour
{
    [SerializeField] private AgencyRelationBinding[] _bindings;

    public void SetAgencyRelation(AgencyData agency)
    {
        if (_bindings == null)
            return;

        foreach (var binding in _bindings)
        {
            if (binding == null || binding.AgencyId != agency.Id)
                continue;

            binding.Set(agency.Name, agency.Relation);
        }
    }
}
