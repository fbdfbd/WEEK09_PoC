using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SynergyDefinition", menuName = "Synergy/Synergy Definition")]
public class SynergyDefinition : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private string _displayName;
    [SerializeField] private List<SynergyCondition> _conditions;
    [SerializeField] private List<SynergyEffect> _effects;

    public string Id => _id;
    public string DisplayName => _displayName;

    public bool CanActivate(BattleContext context)
    {
        if (_conditions == null || _conditions.Count == 0)
        {
            return true;
        }

        for (int i = 0; i < _conditions.Count; i++)
        {
            if (_conditions[i] == null)
            {
                continue;
            }

            if (_conditions[i].IsMet(context) == false)
            {
                return false;
            }
        }

        return true;
    }

    public void Apply(BattleContext context)
    {
        if (_effects == null)
        {
            return;
        }

        for (int i = 0; i < _effects.Count; i++)
        {
            if (_effects[i] != null)
            {
                _effects[i].Apply(context);
            }
        }
    }
}
